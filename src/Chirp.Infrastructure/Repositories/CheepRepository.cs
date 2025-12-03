using Chirp.Core.Data;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Repositories;
using Chirp.Core.Models;

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

        public IEnumerable<Cheep> GetByAuthorEmail(string authorEmail, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.Author.Email == authorEmail)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }


        public async Task DeleteAllCheepsAsync(string id)
        {
            await _context.Cheeps
                .Where(c => c.AuthorId == id)
                .ExecuteDeleteAsync();
        }
        public IEnumerable<Cheep> GetByMultipleAuthors(List<string> authors, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => authors.Contains(c.Author.Name))
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public void AddCheep(string text, string authorId)
        {
            Cheep cheep = new Cheep
            {
                AuthorId = authorId,
                Text = text,
                TimeStamp = DateTime.Now
            };

            _context.Cheeps.Add(cheep);
            _context.SaveChanges();
        }
    }
}