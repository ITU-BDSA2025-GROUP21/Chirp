using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Services
{
    public interface ICheepService
    {
        public IEnumerable<CheepDTO> GetCheeps(int page = 1); 
        public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string author, int page = 1);

        void AddCheep(string text, string authorId);

        Task DeleteAllCheepsAsync(string authorId);
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authorIds, int page = 1);

        public CheepDTO? GetById(int cheepId);

        public void Like(int cheepId, string authorId, bool like);

        Task<Likes> getLikeAsync(int cheepId, string authorId, bool like);
    }
}
