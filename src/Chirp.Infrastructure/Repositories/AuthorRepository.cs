using Chirp.Infrastructure.Data;
using Chirp.Core.Repositories;
using Chirp.Core.Models;

namespace Chirp.Razor.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ChirpDBContext _context;

        public AuthorRepository(ChirpDBContext context)
        {
            _context = context;
        }

        public Author? FindAuthorByName(string name)
        {
            return _context.Authors
                .Where(a => a.Name.ToLower() == name.ToLower())
                .FirstOrDefault();
        }

        public Author? FindAuthorByEmail(string email)
        {
            return _context.Authors
                .Where(a => a.Email.ToLower() == email.ToLower())
                .FirstOrDefault();
        }
    }
}