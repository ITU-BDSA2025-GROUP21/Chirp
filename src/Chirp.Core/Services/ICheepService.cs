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

        public void Like(CheepDTO cheepDTO, string authorId);
        public void Dislike(CheepDTO cheepDTO, string authorId);
        public void unLike(CheepDTO cheepDTO, string authorId);
        public void unDislike(CheepDTO cheepDTO, string authorId);
    }

}
