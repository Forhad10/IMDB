using Dapper;
using IMDB.Business.DTOs;
using IMDB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMDB.Business.Services
{
    public class AdvanceSearchService
    {
        private readonly IMDBDbContext _context;

        public AdvanceSearchService(IMDBDbContext context)
        {
            _context = context;
        }

        

        public async Task<IEnumerable<PersonWordResult>> GetPersonWordsAsync(string actorId, int limit = 10)
        {
            using var connection = _context.Database.GetDbConnection();
            const string sql = @"SELECT word as Word, frequency as Frequency FROM person_words(@ActorId, @Limit)";
            return await connection.QueryAsync<PersonWordResult>(sql, new { ActorId = actorId, Limit = limit });
        }

      public async Task<MovieSearchResponseDto> GetExactMatchTitlesAsync(ExactorBestMatchSearchRequestDto request)
{
    using var connection = _context.Database.GetDbConnection();
    var offset = (request.Page - 1) * request.PageSize;

    // Get total count using the exact match function
    var countSql = @"SELECT COUNT(*) FROM exact_match_titles(@Keywords)";
    var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Keywords = request.Keywords });

    string sql;
    object parameters;

    if (request.UserId.HasValue && request.UserId != Guid.Empty)
    {
        // Query with user-specific data
        sql = @"SELECT t.TitleId, t.PrimaryTitle, t.TitleType, t.Genres, t.AverageRating, t.NumVotes,
                       b.bookmark_id as BookmarkId, b.user_id as UserIdBookmark,
                       r.rating_history_id as RatingHistoryId, r.user_id as UserIdRating,
                       r.rating as UserRating
                FROM (
                    SELECT t.title_id as TitleId,
                           t.primary_title as PrimaryTitle,
                           t.title_type as TitleType,
                           t.genres as Genres,
                           COALESCE(tr.average_rating, 0) as AverageRating,
                           COALESCE(tr.num_votes, 0) as NumVotes
                    FROM exact_match_titles(@Keywords) emt
                    INNER JOIN titles t ON t.title_id = emt.title_id
                    LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                ) t
                LEFT JOIN user_bookmarks b ON t.TitleId = b.title_id AND b.user_id = @UserId
                LEFT JOIN user_rating_history r ON t.TitleId = r.title_id AND r.user_id = @UserId
                ORDER BY t.AverageRating DESC, t.TitleId ASC
                LIMIT @PageSize OFFSET @Offset";

        parameters = new
        {
            Keywords = request.Keywords,
            UserId = request.UserId.Value,
            PageSize = request.PageSize,
            Offset = offset
        };
    }
    else
    {
        // Query without user-specific data
        sql = @"SELECT t.title_id as TitleId,
                       t.primary_title as PrimaryTitle,
                       t.title_type as TitleType,
                       t.genres as Genres,
                       COALESCE(tr.average_rating, 0) as AverageRating,
                       COALESCE(tr.num_votes, 0) as NumVotes
                FROM exact_match_titles(@Keywords) emt
                INNER JOIN titles t ON t.title_id = emt.title_id
                LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                ORDER BY AverageRating DESC, t.title_id ASC
                LIMIT @PageSize OFFSET @Offset";

        parameters = new
        {
            Keywords = request.Keywords,
            PageSize = request.PageSize,
            Offset = offset
        };
    }

    var data = await connection.QueryAsync<MovieSearchResultDto>(sql, parameters);

    return new MovieSearchResponseDto
    {
        Page = request.Page,
        PageSize = request.PageSize,
        TotalCount = totalCount,
        SearchQuery = string.Join(" ", request.Keywords), // Optional: Join keywords for display
        Data = data
    };
}



        public async Task<MovieSearchResponseDto> GetBestMatchTitlesAsync(ExactorBestMatchSearchRequestDto request)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (request.Page - 1) * request.PageSize;

            // Get total count using the exact match function
            var countSql = @"SELECT COUNT(*) FROM best_match_titles(@Keywords)";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { Keywords = request.Keywords });

            string sql;
            object parameters;

            if (request.UserId.HasValue && request.UserId != Guid.Empty)
            {
                // Query with user-specific data
                sql = @"SELECT t.TitleId, t.PrimaryTitle, t.TitleType, t.Genres, t.AverageRating, t.NumVotes,
                       b.bookmark_id as BookmarkId, b.user_id as UserIdBookmark,
                       r.rating_history_id as RatingHistoryId, r.user_id as UserIdRating,
                       r.rating as UserRating
                FROM (
                    SELECT t.title_id as TitleId,
                           t.primary_title as PrimaryTitle,
                           t.title_type as TitleType,
                           t.genres as Genres,
                           COALESCE(tr.average_rating, 0) as AverageRating,
                           COALESCE(tr.num_votes, 0) as NumVotes
                    FROM best_match_titles(@Keywords) emt
                    INNER JOIN titles t ON t.title_id = emt.title_id
                    LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                ) t
                LEFT JOIN user_bookmarks b ON t.TitleId = b.title_id AND b.user_id = @UserId
                LEFT JOIN user_rating_history r ON t.TitleId = r.title_id AND r.user_id = @UserId
                ORDER BY t.AverageRating DESC, t.TitleId ASC
                LIMIT @PageSize OFFSET @Offset";

                parameters = new
                {
                    Keywords = request.Keywords,
                    UserId = request.UserId.Value,
                    PageSize = request.PageSize,
                    Offset = offset
                };
            }
            else
            {
                // Query without user-specific data
                sql = @"SELECT t.title_id as TitleId,
                       t.primary_title as PrimaryTitle,
                       t.title_type as TitleType,
                       t.genres as Genres,
                       COALESCE(tr.average_rating, 0) as AverageRating,
                       COALESCE(tr.num_votes, 0) as NumVotes
                FROM best_match_titles(@Keywords) emt
                INNER JOIN titles t ON t.title_id = emt.title_id
                LEFT JOIN title_ratings tr ON t.title_id = tr.title_id
                ORDER BY AverageRating DESC, t.title_id ASC
                LIMIT @PageSize OFFSET @Offset";

                parameters = new
                {
                    Keywords = request.Keywords,
                    PageSize = request.PageSize,
                    Offset = offset
                };
            }

            var data = await connection.QueryAsync<MovieSearchResultDto>(sql, parameters);

            return new MovieSearchResponseDto
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                SearchQuery = string.Join(" ", request.Keywords), // Optional: Join keywords for display
                Data = data
            };
        }
        //public async Task<IEnumerable<BestMatchTitleResult>> GetBestMatchTitlesAsync(string[] keywords)
        //{
        //    using var connection = _context.Database.GetDbConnection();
        //    const string sql = @"SELECT title_id as TitleId, primary_title as PrimaryTitle, matched_count as MatchedCount, matched_words as MatchedWords FROM best_match_titles(@Keywords)";
        //    return await connection.QueryAsync<BestMatchTitleResult>(sql, new { Keywords = keywords });
        //}

        public async Task<IEnumerable<KeywordWordResult>> GetKeywordWordListAsync(string[] keywords)
        {
            using var connection = _context.Database.GetDbConnection();
            const string sql = @"SELECT word as Word, frequency as Frequency FROM keyword_word_list(@Keywords)";
            return await connection.QueryAsync<KeywordWordResult>(sql, new { Keywords = keywords });
        }
    }
}