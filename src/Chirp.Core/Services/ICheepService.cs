using Chirp.Core.DTO;

namespace Chirp.Core.Services
{
    public interface ICheepService
    {
        public IEnumerable<CheepDTO> GetCheeps(int page = 1); 
        public IEnumerable<CheepDTO> GetCheepsFromAuthorEmail(string author, int page = 1);

        void AddCheeps(string text, string authorId);

        Task DeleteAllCheepsAsync(string authorId);
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authors, int page = 1);
    }
}
