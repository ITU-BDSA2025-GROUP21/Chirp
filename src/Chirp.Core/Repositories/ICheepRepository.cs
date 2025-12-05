using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<Cheep> GetByAuthorId(string authorId, int page = 1, int pageSize = 32);

        public void AddCheep(string text, string authorId);
        IEnumerable<Cheep> GetByMultipleAuthors(List<string> authorIds, int page = 1, int pageSize = 32);

        public void Like(int cheepId,  string authorID, bool like);

        public Cheep GetById(int id);

        Task<Likes> GetLikeAsync(int cheepId, string authorId, bool like);
    }
}