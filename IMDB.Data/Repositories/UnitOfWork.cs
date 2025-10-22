using IMDB.Data.Entities;

namespace IMDB.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMDBDbContext _context;
        private IGenericRepository<Title>? _titles;

        public UnitOfWork(IMDBDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Title> Titles => _titles ??= new GenericRepository<Title>(_context);

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
    }
}
