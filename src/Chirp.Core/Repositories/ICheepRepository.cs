using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<Cheep> GetByAuthorId(string authorId, int page = 1, int pageSize = 32);

        public void AddCheep(string text, string authorId);
        Task DeleteAllCheepsAsync(string authorId);

        IEnumerable<Cheep> GetByMultipleAuthors(List<string> authorIds, int page = 1, int pageSize = 32);

        public void Like(Cheep cheep,  string authorID);
        public void Dislike(Cheep cheep, string authorID);

        public Cheep GetById(int id);
    }
}