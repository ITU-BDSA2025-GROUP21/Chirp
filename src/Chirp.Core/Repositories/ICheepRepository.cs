using Chirp.Core.DTO;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        IEnumerable<CheepDTO> GetAll(int page = 1, int pageSize = 32);
        IEnumerable<CheepDTO> GetByAuthor(string authorName, int page = 1, int pageSize = 32);
        public void CreateNewAuthor(string name, string email, string password);
        public AuthorDTO FindAuthorByName(string name);
        public AuthorDTO FindAuthorByEmail(string email);
        public void AddChirp(CheepDTO chirp);
        void Save();
    }
}