using IMDB.Data.Entities;
using IMDB.Business.DTOs;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Business.Services
{
    public class MovieSearchService
    {
        private readonly IMDBDbContext _context;

        public MovieSearchService(IMDBDbContext context)
        {
            _context = context;
        }

        public async Task<MovieSearchResponseDto> BasicSearchAsync(BasicMovieSearchRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get total count using the search function
            var countSql = @"SELECT COUNT(*) FROM search_titles(@SearchQuery)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { SearchQuery = request.SearchQuery });

            // Get paginated search results
            var sql = @"SELECT t.title_id as TitleId,
                              t.primary_title as PrimaryTitle,
                              t.title_type as TitleType,
                              t.genres as Genres,
                              COALESCE(tr.average_rating, 0) as AverageRating,
                              COALESCE(tr.num_votes, 0) as NumVotes
                       FROM search_titles(@SearchQuery) st
                       INNER JOIN titles t ON t.title_id = st.title_id
                       LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                       ORDER BY AverageRating DESC, t.title_id ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                SearchQuery = request.SearchQuery,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<MovieSearchResultDto>(sql, parameters);

            return new MovieSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = request.SearchQuery,
                Data = data
            };
        }

        public async Task<MovieSearchResponseDto> StructuredSearchAsync(StructuredMovieSearchRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get total count using the structured search function
            var countSql = @"SELECT COUNT(*) FROM structured_string_search(@Title, @Plot, @Characters, @Person)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new
            {
                Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title,
                Plot = string.IsNullOrWhiteSpace(request.Plot) ? null : request.Plot,
                Characters = string.IsNullOrWhiteSpace(request.Characters) ? null : request.Characters,
                Person = string.IsNullOrWhiteSpace(request.Person) ? null : request.Person
            });

            // Get paginated structured search results
            var sql = @"SELECT t.title_id as TitleId,
                              t.primary_title as PrimaryTitle,
                              t.title_type as TitleType,
                              t.genres as Genres,
                              COALESCE(tr.average_rating, 0) as AverageRating,
                              COALESCE(tr.num_votes, 0) as NumVotes
                       FROM structured_string_search(@Title, @Plot, @Characters, @Person) sss
                       INNER JOIN titles t ON t.title_id = sss.title_id
                       LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                       ORDER BY AverageRating DESC, t.title_id ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title,
                Plot = string.IsNullOrWhiteSpace(request.Plot) ? null : request.Plot,
                Characters = string.IsNullOrWhiteSpace(request.Characters) ? null : request.Characters,
                Person = string.IsNullOrWhiteSpace(request.Person) ? null : request.Person,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<MovieSearchResultDto>(sql, parameters);

            return new MovieSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = request.Title,
                TitleFilter = request.Title,
                PlotFilter = request.Plot,
                CharactersFilter = request.Characters,
                PersonFilter = request.Person,
                Data = data
            };
        }

        public async Task<MovieSearchResponseDto> GetSimilarMoviesAsync(SimilarMoviesRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get the base movie title for reference
            var baseMovieSql = @"SELECT primary_title FROM titles WHERE title_id = @TitleId";
            var baseMovieTitle = await connection.ExecuteScalarAsync<string>(baseMovieSql, new { TitleId = request.TitleId });

            // Get total count using the similar movies function
            var countSql = @"SELECT COUNT(*) FROM similar_movies(@TitleId)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { TitleId = request.TitleId });

            // Get paginated similar movies results
            var sql = @"SELECT t.title_id as TitleId,
                              t.primary_title as PrimaryTitle,
                              t.title_type as TitleType,
                              t.genres as Genres,
                              COALESCE(tr.average_rating, 0) as AverageRating,
                              COALESCE(tr.num_votes, 0) as NumVotes
                       FROM similar_movies(@TitleId) sm
                       INNER JOIN titles t ON t.title_id = sm.title_id
                       LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                       ORDER BY AverageRating DESC, t.title_id ASC
                       LIMIT @PageSize OFFSET @Offset";

            var parameters = new
            {
                TitleId = request.TitleId,
                PageSize = request.PageSize,
                Offset = offset
            };

            var data = await connection.QueryAsync<MovieSearchResultDto>(sql, parameters);

            return new MovieSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                BaseTitleId = request.TitleId,
                SearchQuery = baseMovieTitle,
                Data = data
            };
        }
    }
}
