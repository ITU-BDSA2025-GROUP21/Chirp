using Chirp.Core.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Core.Models;
using Chirp.Core.DTO;

namespace Chirp.Razor.Repositories
{
    public class CheepRepository : ICheepRepository
    {
        private readonly ChirpDBContext _context;

        public CheepRepository(ChirpDBContext context)
        {
            _context = context;
        }

        public IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public IEnumerable<Cheep> GetByAuthor(string authorName, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.Author.Name == authorName)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public void AddCheep(string text, Author author) {

            Cheep cheep = new Cheep
            {
                Author = author,
                TimeStamp = DateTime.Now,
                Text = text
            };

            author.Cheeps.Add(cheep);
            _context.Cheeps.Add(cheep);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}