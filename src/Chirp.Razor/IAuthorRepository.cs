using Chirp.Razor.Data;

namespace Chirp.Razor.Repositories
{
    public interface IAuthorRepository
    {
        Author? GetByName(string name);
        void Add(Author author);
        void Save();
    }
}