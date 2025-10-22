using Chirp.Razor.Data;
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

        public Author? GetByName(string name)
        {
            return _context.Authors.FirstOrDefault(a => a.Name == name);
        }

        public void Add(Author author)
        {
            _context.Authors.Add(author);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
