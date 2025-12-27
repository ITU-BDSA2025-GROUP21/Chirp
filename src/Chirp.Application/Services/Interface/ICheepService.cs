using Chirp.Application.DTO;
using Chirp.Core.Models;

namespace Chirp.Application.Services.Interface
{
    public interface ICheepService
    {
        public IEnumerable<CheepDTO> GetCheeps(int page = 1);
        public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string author, int page = 1);

        void AddCheep(string text, string authorId);
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authorIds, int page = 1);

        public CheepDTO? GetById(int cheepId);

        public void Like(int cheepId, string authorId, bool like);

        Task<Like> GetLikeAsync(int cheepId, string authorId, bool like);
    }
}
