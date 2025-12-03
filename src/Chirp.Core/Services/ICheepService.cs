using Chirp.Core.DTO;

namespace Chirp.Core.Services
{
    public interface ICheepService
    {
        public IEnumerable<CheepDTO> GetCheeps(int page = 1); 
        public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int page = 1);
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authors, int page = 1);
        public void MakeCheep(CheepDTO cheep);
    }
}
