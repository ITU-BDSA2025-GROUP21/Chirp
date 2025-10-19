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

            //string dataDump = Environment.GetEnvironmentVariable("CHIRPDATADUMP");
            
            repository = new CheepRepository();
            
            //ImportDataDump(dataDump);
        }

        public void AddCheep(Author a, String t)
        {
            var cheep = new Cheep()
            {
                author = a,
                text = t,
                timestamp = DateTime.Now
            };
            repository.Add(cheep);
        }
        
        public void ImportDataDump(string dataDump)
        {
            if (string.IsNullOrWhiteSpace(dataDump) || !File.Exists(dataDump))
                return;

            string sql = File.ReadAllText(dataDump);

            using var context = new CheepRepository();
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var statements = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);

                foreach (var statement in statements)
                {
                    var trimmed = statement.Trim();
                    if (!string.IsNullOrEmpty(trimmed))
                    {
                        context.Database.ExecuteSqlRaw(trimmed);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error importing SQL dump: {ex.Message}");
            }
        }


        public List<CheepViewModel> GetCheepPage(int page = 1)
        {
            int offset = (page - 1) * PageSize;

            var cheeps = repository.cheeps
                .Include(c => c.author)
                .OrderByDescending(c => c.timestamp)
                .Skip(offset)
                .Take(PageSize)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.author.name,
                    c.text,
                    c.timestamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string authorName, int page = 1)
        {
            int offset = (page - 1) * PageSize;

            var cheeps = repository.cheeps
                .Include(c => c.author)
                .Where(c => c.author.name == authorName)
                .OrderByDescending(c => c.timestamp)
                .Skip(offset)
                .Take(PageSize)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.author.name,
                    c.text,
                    c.timestamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }

        private List<CheepViewModel> GetCheeps()
        {
            var cheeps = repository.cheeps
                .Include(c => c.author)
                .OrderByDescending(c => c.timestamp)
                .AsNoTracking()
                .Select(c => new CheepViewModel(
                    c.author.name,
                    c.text,
                    c.timestamp.ToString("MM/dd/yy H:mm:ss")
                ))
                .ToList();

            return cheeps;
        }

        private SqliteDataReader ExecuteReader(string connectionString, string query, Dictionary<string, object>? parameters = null)
        {
            var conn = new SqliteConnection(connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = query;

            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    cmd.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                }
            }

            var reader = cmd.ExecuteReader();

            return reader;
        }


        private void ExecuteNonQuery(string connectionString, params string[] queries)
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    foreach (string query in queries)
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
