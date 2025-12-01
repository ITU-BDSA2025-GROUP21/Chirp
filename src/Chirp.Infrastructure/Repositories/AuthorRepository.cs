using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task DeleteAuthorByIdAsync(string authorId)
        {
            await _context.Authors
                .Where(a => a.Id == authorId)
                .ExecuteDeleteAsync();
        }
    }
}