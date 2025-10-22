using Chirp.Razor.Data;

namespace Chirp.Razor.Repositories
{
    public interface ICheepRepository
    {
        void Add(Cheep cheep);
        IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<Cheep> GetByAuthor(string authorName, int page = 1, int pageSize = 32);
        void Save();
    }
}