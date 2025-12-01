using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;

namespace Chirp.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;

        public AuthorService(IAuthorRepository repo) {
            _repo = repo;
        }

        public AuthorDTO? FindAuthorByName(string name)
        {
            return CreateAuthorDTO(_repo.FindAuthorByName(name));
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            return CreateAuthorDTO(_repo.FindAuthorByEmail(email));
        }


        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if(author == null)
            {
                return null;
            }

            return new AuthorDTO
            {
                Name = author.Name,
                Email = author.Email
            };
        }
    }
}

