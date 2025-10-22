// File: CheepRepository.cs
using Chirp.Razor.Data;
using Microsoft.EntityFrameworkCore;

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

        public IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .Include(c => c.Author)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Cheep> GetByAuthor(string authorName, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .Include(c => c.Author)
                .Where(c => c.Author.Name == authorName)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .AsNoTracking()
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
