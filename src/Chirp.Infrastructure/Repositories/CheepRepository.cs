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
                .OrderByDescending(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public IEnumerable<Cheep> GetByAuthorId(string authorId, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.AuthorId == authorId)
                .OrderBy(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public Cheep GetById(int id)
        {
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .FirstOrDefault(c => c.CheepId == id);
        }

        public async Task DeleteAllCheepsAsync(string id)
        {
            await _context.Cheeps
                .Where(c => c.AuthorId == id)
                .ExecuteDeleteAsync();
        }
        public IEnumerable<Cheep> GetByMultipleAuthors(List<string> authorIds, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => authorIds.Contains(c.AuthorId))
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

        public void like(string AuthorId, Cheep cheep)
        {

        }

        public void dislike(string AuthorId, Cheep cheep)
        {

        }

        public void Like(Cheep cheep, string authorID)
        {
            throw new NotImplementedException();
        }

        public void Dislike(Cheep cheep, string authorID)
        {
            throw new NotImplementedException();
        }
    }
}