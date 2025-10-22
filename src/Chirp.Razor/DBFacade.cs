using Chirp.Razor.Data;
using Chirp.Razor.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.DBFacade
{
    public class DBFacade
    {
        private const int PageSize = 32;

        private readonly ICheepRepository _cheepRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ChirpDBContext _context;

        public DBFacade()
        {
            _context = new ChirpDBContext();
            _cheepRepository = new CheepRepository(_context);
            _authorRepository = new AuthorRepository(_context);

            _context.Database.CloseConnection();
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            DbInitializer.SeedDatabase(_context);
        }

        public void ResetDB()
        {
            _context.Database.CloseConnection();
            _context.Database.EnsureDeleted();
        }

        public void AddCheep(Author author, string text)
        {
            var cheep = new Cheep
            {
                Author = author,
                Text = text,
                TimeStamp = DateTime.Now
            };

            _cheepRepository.Add(cheep);
            _cheepRepository.Save();
        }

        public List<CheepViewModel> GetCheepPage(int page = 1)
        {
            return _cheepRepository.GetAll(page, PageSize)
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string authorName, int page = 1)
        {
            return _cheepRepository.GetByAuthor(authorName, page, PageSize)
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();
        }

        public List<CheepViewModel> GetCheeps()
        {
            return _cheepRepository.GetAll(1, int.MaxValue)
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();
        }
    }
}
