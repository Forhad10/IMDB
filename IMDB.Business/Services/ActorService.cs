using IMDB.Data.Entities;
using IMDB.Business.DTOs;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Business.Services
{
    public class ActorService
    {
        private readonly IMDBDbContext _context;

        public ActorService(IMDBDbContext context)
        {
            _context = context;
        }

        public async Task<ActorSearchResponseDto> SearchActorsAsync(SearchActorsRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get total count using the search function
            var countSql = @"SELECT COUNT(*) FROM search_names(@SearchQuery)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { SearchQuery = request.SearchQuery });

            // Get paginated search results
            var sql = @"SELECT a.name_id as NameId,
                              a.primary_name as PrimaryName,
                              a.birth_year as BirthYear,
                              a.death_year as DeathYear,
                              a.primary_profession as PrimaryProfession,
                              COALESCE(ar.weighted_rating, 0) as WeightedRating,
                              COALESCE(ar.total_votes, 0) as TotalVotes
                       FROM search_names(@SearchQuery) sn
                       INNER JOIN actors a ON a.name_id = sn.name_id
                       LEFT JOIN actors_ratings ar ON a.name_id = ar.name_id
                       ORDER BY
                           CASE WHEN ar.weighted_rating IS NOT NULL THEN ar.weighted_rating ELSE 0 END DESC,
                           a.primary_name ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                SearchQuery = request.SearchQuery,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<ActorDto>(sql, parameters);

            return new ActorSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = request.SearchQuery,
                Data = data
            };
        }

        public async Task<ActorSearchResponseDto> GetCoPlayersAsync(CoPlayersRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get the base actor name for reference
            var baseActorSql = @"SELECT primary_name FROM actors WHERE name_id = @ActorId";
            var baseActorName = await connection.ExecuteScalarAsync<string>(baseActorSql, new { ActorId = request.ActorId });

            // Get total count using the co-players function
            var countSql = @"SELECT COUNT(*) FROM co_players(@ActorId)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { ActorId = request.ActorId });

            // Get paginated co-players results
            var sql = @"SELECT a.name_id as NameId,
                              a.primary_name as PrimaryName,
                              a.birth_year as BirthYear,
                              a.death_year as DeathYear,
                              a.primary_profession as PrimaryProfession,
                              COALESCE(ar.weighted_rating, 0) as WeightedRating,
                              COALESCE(ar.total_votes, 0) as TotalVotes,
                              cp.frequency as Frequency
                       FROM co_players(@ActorId) cp
                       INNER JOIN actors a ON a.name_id = cp.name_id
                       LEFT JOIN actors_ratings ar ON a.name_id = ar.name_id
                       ORDER BY cp.frequency DESC, ar.weighted_rating DESC NULLS LAST
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                ActorId = request.ActorId,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<ActorDto>(sql, parameters);

            return new ActorSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = baseActorName,
                ActorId = request.ActorId,
                Data = data
            };
        }

        public async Task<ActorSearchResponseDto> GetPopularActorsAsync(PopularActorsRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get the movie title for reference
            var movieSql = @"SELECT primary_title FROM titles WHERE title_id = @TitleId";
            var movieTitle = await connection.ExecuteScalarAsync<string>(movieSql, new { TitleId = request.TitleId });

            // Get total count using the popular actors function
            var countSql = @"SELECT COUNT(*) FROM popular_actors(@TitleId)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { TitleId = request.TitleId });

            // Get paginated popular actors results
            var sql = @"SELECT DISTINCT a.name_id as NameId,
                              a.primary_name as PrimaryName,
                              a.birth_year as BirthYear,
                              a.death_year as DeathYear,
                              a.primary_profession as PrimaryProfession,
                              pa.weighted_rating as WeightedRating,
                              COALESCE(ar.total_votes, 0) as TotalVotes
                       FROM popular_actors(@TitleId) pa
                       INNER JOIN title_principals tp ON tp.title_id = @TitleId AND tp.actor_id = pa.actor_id
                       INNER JOIN actors a ON a.name_id = tp.actor_id
                       LEFT JOIN actors_ratings ar ON a.name_id = ar.name_id
                       ORDER BY pa.weighted_rating DESC NULLS LAST, a.primary_name ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                TitleId = request.TitleId,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<ActorDto>(sql, parameters);

            return new ActorSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = movieTitle,
                TitleId = request.TitleId,
                Data = data
            };
        }

        public async Task<ActorSearchResponseDto> GetAllActorsAsync(GetAllActorsRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();

            // Get paginated actor IDs for rating updates
            var offset = (request.Page - 1) * request.PageSize;
            var actorIdsSql = @"SELECT name_id FROM actors ORDER BY primary_name LIMIT @PageSize OFFSET @Offset";
            var actorIds = await connection.QueryAsync<string>(actorIdsSql, new { PageSize = request.PageSize, Offset = offset });

            // Call update_actor_ratings for each actor
            foreach (var actorId in actorIds)
            {
                await connection.ExecuteAsync("SELECT update_actor_ratings(@ActorId)", new { ActorId = actorId });
            }

            // Get total count
            var countSql = @"SELECT COUNT(*) FROM actors";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            // Get paginated actors
            var sql = @"SELECT a.name_id as NameId,
                              a.primary_name as PrimaryName,
                              a.birth_year as BirthYear,
                              a.death_year as DeathYear,
                              a.primary_profession as PrimaryProfession,
                              COALESCE(ar.weighted_rating, 0) as WeightedRating,
                              COALESCE(ar.total_votes, 0) as TotalVotes
                       FROM actors a
                       LEFT JOIN actors_ratings ar ON a.name_id = ar.name_id
                       ORDER BY a.primary_name ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<ActorDto>(sql, parameters);

            return new ActorSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

    }
}
