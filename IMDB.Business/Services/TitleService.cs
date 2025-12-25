using IMDB.Data.Entities;
using IMDB.Business.DTOs;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Business.Services
{
    public class TitleService
    {
        private readonly IMDBDbContext _context;

        public TitleService(IMDBDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedTitleResponseDto> GetAllAsync(int page, int pageSize)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (page - 1) * pageSize;

            // Get total count first
            var countSql = @"select
                       count(*)
                       from titles t
                       left join title_ratings tr on t.title_id = tr.title_id";
            var totalCount = await connection.ExecuteScalarAsync<int>(countSql);

            // Get paginated data
            var sql = @"select
                       t.title_id as TitleId,
                       t.primary_title as PrimaryTitle,
                       t.title_type as TitleType,
                       t.genres as Genres,
                       COALESCE(tr.average_rating, 0) as AverageRating,
                       COALESCE(tr.num_votes, 0) as NumVotes
                       from titles t
                       left join title_ratings tr on t.title_id = tr.title_id
                      ORDER BY AverageRating DESC, t.title_id ASC
                      OFFSET @Offset ROWS
                      FETCH NEXT @PageSize ROWS ONLY";
            var parameters = new { Offset = offset, PageSize = pageSize };
            var data = await connection.QueryAsync<TitleDto>(sql, parameters);

            return new PaginatedTitleResponseDto
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };
        }

        public async Task<PaginatedTitleResponseDto> GetAllWithUserAsync(Guid userId, int page, int pageSize)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (page - 1) * pageSize;

            // Get total count of titles the user has interacted with
            var countSql = @"select
                       count(*)
                       from titles t
                       left join title_ratings tr on t.title_id = tr.title_id";

            var totalCount = await connection.ExecuteScalarAsync<int>(countSql, new { UserId = userId });


            try
            {   // Get paginated data of titles the user has interacted with
                var sql = @"select t.TitleId,t.PrimaryTitle,t.TitleType,t.Genres,t.AverageRating,t.NumVotes,
                       b.bookmark_id  BookmarkId,b.user_id UserIdBookmark,
                       C.rating_history_id RatingHistoryId, c.user_id UserIdRating
                         from  
                       (select
                       t.title_id as TitleId,
                       t.primary_title as PrimaryTitle,
                       t.title_type as TitleType,
                       t.genres as Genres,
                       COALESCE(tr.average_rating, 0) as AverageRating,
                       COALESCE(tr.num_votes, 0) as NumVotes
                       from titles t
                       left join title_ratings tr on t.title_id = tr.title_id) t

					   left join
					   
					   (
                               select * from user_bookmarks where user_id=@UserId
							   
					   )  
					   b on t.TitleId=b.title_id

					    left join
					   
					   (
                               select * from user_rating_history  where user_id=@UserId
							   
					   )  
					   c on t.TitleId=c.title_id

					   
                      ORDER BY t.AverageRating DESC, t.TitleId ASC
                       OFFSET @Offset ROWS
                       FETCH NEXT @PageSize ROWS ONLY";

                var parameters = new { UserId = userId, Offset = offset, PageSize = pageSize };
                var data = await connection.QueryAsync<TitleDto>(sql, parameters);

                return new PaginatedTitleResponseDto
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    Data = data
                };
            }


            catch (System.Exception ex)
            {
                throw new System.Exception("Error fetching titles with user data", ex.InnerException);

            }
        }
    }
}
