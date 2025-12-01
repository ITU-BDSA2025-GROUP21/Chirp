using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface IAuthorRepository
    {
        public Author? FindAuthorByName(string name);
        public Author? FindAuthorByEmail(string email);

        Task DeleteAuthorByIdAsync(string authorId);

    }
}