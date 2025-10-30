using Chirp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Chirp.Core.Repositories;
using Chirp.Core.DTO;
using Chirp.Infrastructure.Models;

namespace Chirp.Razor.Repositories
{
    public class CheepRepository : ICheepRepository
    {
        private readonly ChirpDBContext _context;

        public CheepRepository(ChirpDBContext context)
        {
            _context = context;
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
