// File: CheepRepository.cs
using Chirp.Razor.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chirp.Razor.Repositories
{
    public class CheepRepository : ICheepRepository
    {
        private readonly ChirpDBContext _context;

        public CheepRepository(ChirpDBContext context)
        {
            _context = context;
        }

        public void Add(Cheep cheep)
        {
            _context.Cheeps.Add(cheep);
        }

        public IEnumerable<CheepDTO> GetAll(int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .Select(createCheepDTO)
                .ToList();
        }

        public IEnumerable<CheepDTO> GetByAuthor(string authorName, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Where(c => c.Author.Name == authorName)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .Select(createCheepDTO)
                .ToList();
        }

        public void CreateNewAuthor(string name, string email)
        {
            using (var context = new YourDbContext())
            {
                var author = new Author
                {
                    Name = name,
                    Email = email,
                    Cheeps = new List<Cheep>()
                };

                context.Authors.Add(author);
                context.SaveChanges();
            }
        }

        public Author FindAuthorByName(string name)
        {
            using (var context = new YourDbContext())
            {
                return context.Authors
                            .FirstOrDefault(a => a.Name.ToLower() == name.ToLower());
            }
        }

        public Author FindAuthorByEmail(string email)
        {
            using (var context = new YourDbContext())
            {
                return context.Authors
                            .FirstOrDefault(a => a.Email.ToLower() == email.ToLower());

            }
        }

        public void AddChirp(CheepDTO chirp)
        {
            
        }

        private readonly Expression<Func<Cheep, CheepDTO>> createCheepDTO =
            c => new CheepDTO
            {
                Author = c.Author.Name,
                Message = c.Text,
                CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
            };

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
