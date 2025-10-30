using Chirp.Core.DTO;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        IEnumerable<CheepDTO> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<CheepDTO> GetByAuthor(string authorName, int page = 1, int pageSize = 32);
        void Save();
    }
}