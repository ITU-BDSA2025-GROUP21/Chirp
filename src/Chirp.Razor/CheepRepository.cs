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

        public IEnumerable<CheepDTO> GetAll(int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .Select(c => new CheepDTO
                {
                    Author = c.Author.Name,
                    Message = c.Text,
                    CreatedDate = c.TimeStamp.ToString("yyyy-MM-dd HH:mm")
                })
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
                .Select(c => new CheepDTO
                {
                    Author = c.Author.Name,
                    Message = c.Text,
                    CreatedDate = c.TimeStamp.ToString("yyyy-MM-dd HH:mm")
                })
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
