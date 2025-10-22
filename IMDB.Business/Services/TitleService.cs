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

        public async Task<IEnumerable<TitleDto>> GetAllAsync(int page, int pageSize)
        {
            using var connection = _context.Database.GetDbConnection();
            var offset = (page - 1) * pageSize;
            var sql = @"select 
                       t.title_id as TitleId,
                       t.primary_title as PrimaryTitle,
                       t.title_type as TitleType,
                       t.genres as Genres,
                       COALESCE(tr.average_rating, 0) as AverageRating,
                       COALESCE(tr.num_votes, 0) as NumVotes
                       from titles t
                       left join title_ratings tr on t.title_id = tr.title_id
                       ORDER BY AverageRating desc OFFSET @Offset LIMIT @PageSize";
            var parameters = new { Offset = offset, PageSize = pageSize };
            return await connection.QueryAsync<TitleDto>(sql, parameters);
        }
    }
}
