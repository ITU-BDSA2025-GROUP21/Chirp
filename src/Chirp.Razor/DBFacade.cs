using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using Chirp.Razor.Data;
using Microsoft.EntityFrameworkCore;


namespace Chirp.Razor.DBFacade
{
    public class DBFacade
    {
        private const int PageSize = 32;

        private CheepRepository repository;

        public DBFacade()
        {

            string dataDump = "dump.sql";
            
            repository = new CheepRepository();
            repository.Database.EnsureDeleted();
            repository.Database.EnsureCreated();
            
            DbInitializer.SeedDatabase(repository);
        }

        public void AddCheep(Author a, String t)
        {
            var cheep = new Cheep()
            {
                Author = a,
                Text = t,
                TimeStamp = DateTime.Now
            };
            repository.Add(cheep);
            repository.SaveChanges();
        }

        public List<CheepViewModel> GetCheepPage(int page = 1)
        {
            int offset = (page - 1) * PageSize;

            var cheeps = repository.Cheeps
                .Include(c => c.Author)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(offset)
                .Take(PageSize)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string authorName, int page = 1)
        {
            int offset = (page - 1) * PageSize;

            var cheeps = repository.Cheeps
                .Include(c => c.Author)
                .Where(c => c.Author.Name == authorName)
                .OrderByDescending(c => c.TimeStamp)
                .Skip(offset)
                .Take(PageSize)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }

        private List<CheepViewModel> GetCheeps()
        {
            var cheeps = repository.Cheeps
                .Include(c => c.Author)
                .OrderByDescending(c => c.TimeStamp)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.Author.Name,
                    c.Text,
                    c.TimeStamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }
        
        
    }
}
