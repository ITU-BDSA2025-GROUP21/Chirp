using Chirp.Core.DTO;

namespace Chirp.Core.Services
{
    public interface ICheepService
    {
        public IEnumerable<CheepDTO> GetCheeps(int page = 1); 
        public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string author, int page = 1);

        void AddCheep(string text, string authorId);

        Task DeleteAllCheepsAsync(string authorId);
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authorIds, int page = 1);

        public void Like(int cheepId, string authorId);
        public void Dislike(int cheepId, string authorId);
        public void unLike(int cheepId, string authorId);
        public void unDislike(int cheepId, string authorId);
    }

}
