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

        public async Task<IEnumerable<ExactMatchTitleResult>> GetExactMatchTitlesAsync(string[] keywords)
        {
            using var connection = _context.Database.GetDbConnection();
            const string sql = @"SELECT title_id as TitleId, primary_title as PrimaryTitle FROM exact_match_titles(@Keywords)";
            return await connection.QueryAsync<ExactMatchTitleResult>(sql, new { Keywords = keywords });
        }

        public async Task<IEnumerable<BestMatchTitleResult>> GetBestMatchTitlesAsync(string[] keywords)
        {
            using var connection = _context.Database.GetDbConnection();
            const string sql = @"SELECT title_id as TitleId, primary_title as PrimaryTitle, matched_count as MatchedCount, matched_words as MatchedWords FROM best_match_titles(@Keywords)";
            return await connection.QueryAsync<BestMatchTitleResult>(sql, new { Keywords = keywords });
        }

        public async Task<IEnumerable<KeywordWordResult>> GetKeywordWordListAsync(string[] keywords)
        {
            using var connection = _context.Database.GetDbConnection();
            const string sql = @"SELECT word as Word, frequency as Frequency FROM keyword_word_list(@Keywords)";
            return await connection.QueryAsync<KeywordWordResult>(sql, new { Keywords = keywords });
        }
    }
}