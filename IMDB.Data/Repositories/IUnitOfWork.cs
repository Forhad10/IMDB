using IMDB.Data.Entities;
using System.Threading.Tasks;

namespace IMDB.Data.Repositories
{
    public interface IUnitOfWork
    {
        IGenericRepository<Title> Titles { get; }   
        Task<int> SaveAsync();
    }
}
