using Dapper;
using IMDB.Data;
using IMDB.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace IMDB.Business.Services
{
    public class DropdownListService
    {
        private readonly IMDBDbContext _context;

        public DropdownListService(IMDBDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActorDropdownDto>> GetActorsForDropdownAsync()
        {
            using var connection = _context.Database.GetDbConnection();

            const string query = @"
                SELECT 
                    name_id AS NameId, 
                    primary_name AS PrimaryName
                FROM actors";

            return await connection.QueryAsync<ActorDropdownDto>(query);
        }
    }

    public class ActorDropdownDto
    {
        public string NameId { get; set; } = null!;
        public string? PrimaryName { get; set; }
    }
}