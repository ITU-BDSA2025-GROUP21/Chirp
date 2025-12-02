using Chirp.Core.Data;
using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
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

        public IEnumerable<Cheep> GetByAuthor(string authorEmail, int page = 1, int pageSize = 32)
        {
            int offset = (page - 1) * pageSize;
            return _context.Cheeps
                .AsNoTracking()
                .Include(c => c.Author)
                .Where(c => c.Author.Email.ToLower() == authorEmail.ToLower())
                .OrderByDescending(c => c.TimeStamp)
                .Skip(offset)
                .Take(pageSize)
                .ToList();
        }

        public void AddCheep(string text, Author author) {

            _context.Authors.Add(author);
            _context.SaveChanges();
            
        }

        public AuthorDTO? FindAuthorByName(string email)
        {
            return _context.Authors
                .Where(a => a.Email.ToLower() == email.ToLower())
                .Select(a => new AuthorDTO
                {
                    Name = a.Name,
                    Email = a.Email,
                })
                .FirstOrDefault();
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            return _context.Authors
                .Where(a => a.Email.ToLower() == email.ToLower())
                .Select(a => new AuthorDTO
                {
                    Name = a.Name,
                    Email = a.Email,
                })
                .FirstOrDefault();
        }

        public void AddChirp(CheepDTO chirp)
        {
            var author = _context.Authors.FirstOrDefault(a => a.Email == chirp.Author);
            if (author == null)
            {
                throw new InvalidOperationException($"No author found with name '{chirp.Author}'.");
            }

            if (!DateTime.TryParse(chirp.CreatedDate, out var parsedDate))
            {
                parsedDate = DateTime.Now;
            }


            var cheep = new Cheep
            {
                AuthorId = author.Id,
                Text = chirp.Message,
                TimeStamp = parsedDate
            };

            author.Cheeps.Add(cheep);
            _context.Cheeps.Add(cheep);
            _context.SaveChanges();
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