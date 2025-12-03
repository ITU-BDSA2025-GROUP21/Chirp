using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<Cheep> GetByAuthor(string authorName, int page = 1, int pageSize = 32);
        IEnumerable<Cheep> GetByMultipleAuthors(List<string> authors, int page = 1, int pageSize = 32);
        public void AddCheep(string text, Author author);
        void Save();
    }
}