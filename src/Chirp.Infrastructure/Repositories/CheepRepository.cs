using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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

        public Cheep? GetById(int id)
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

        public void Like(int cheepId, string authorID, bool like)
        {
            var cheepExists = _context.Cheeps.Any(c => c.CheepId == cheepId);
            if (!cheepExists) return; // or throw a meaningful exception

            // Check if the Author exists
            var authorExists = _context.Authors.Any(a => a.Id == authorID);
            if (!authorExists) return; // or throw a meaningful exception

            // Try to find an existing Like
            var existing = _context.Likes
                .FirstOrDefault(l => l.CheepId == cheepId && l.authorId == authorID);

            if (existing == null)
            {
                _context.Likes.Add(new Likes
                {
                    CheepId = cheepId,
                    authorId = authorID,
                    likeStatus = like ? 1 : -1
                });
            }
            else
            {
                existing.likeStatus = like ? 1 : -1;
            }

            _context.SaveChanges();
        }
        

        public void unLike(int cheepId, string authorID)
        {
            
        }
    }
}