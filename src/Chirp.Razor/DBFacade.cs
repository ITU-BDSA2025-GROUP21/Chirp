using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Chirp.Razor.DBFacade
{
    public class DBFacade
    {
        private readonly string connectionString;
        private const int PageSize = 32;

        public DBFacade()
        {
            var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");
            if (string.IsNullOrEmpty(dbPath))
            {
                dbPath = Path.Combine(Path.GetTempPath(), "chirp.db");
            }
            
            connectionString = $"Data Source={dbPath}";
            initDB();
        }

        public void initDB()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();

                string createUser = @"
                    CREATE TABLE IF NOT EXISTS user (
                    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username STRING NOT NULL,
                    email STRING NOT NULL,
                    pw_hash STRING NOT NULL
                    );";

                string createMessage = @"
                    CREATE TABLE IF NOT EXISTS message (
                    message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    author_id INTEGER NOT NULL,
                    text STRING NOT NULL,
                    pub_date INTEGER
                    );";

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = createUser;
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = createMessage;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ImportSqlDump(string dumpPath)
        {
            if (string.IsNullOrEmpty(dumpPath) || !File.Exists(dumpPath))
            {
                throw new FileNotFoundException($"SQL dump file '{dumpPath}' does not exist.");
            }

            string sql = File.ReadAllText(dumpPath);

            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            using var trans = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new InvalidOperationException($"Failed to import SQL dump: {ex.Message}", ex);
            }
        }


        public List<CheepViewModel> GetCheeps(int page = 1)
        {
            var cheeps = new List<CheepViewModel>();
            
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            var offset = (page - 1) * PageSize;
            var sql = @"
                SELECT u.username, m.text, m.pub_date 
                FROM message m 
                JOIN user u ON m.author_id = u.user_id 
                ORDER BY m.pub_date DESC 
                LIMIT @pageSize OFFSET @offset";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@pageSize", PageSize);
            cmd.Parameters.AddWithValue("@offset", offset);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var author = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = UnixTimeStampToDateTimeString(reader.GetInt64(2));
                
                cheeps.Add(new CheepViewModel(author, message, timestamp));
            }

            return cheeps;
        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1)
        {
            var cheeps = new List<CheepViewModel>();
            
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            var offset = (page - 1) * PageSize;
            var sql = @"
                SELECT u.username, m.text, m.pub_date 
                FROM message m 
                JOIN user u ON m.author_id = u.user_id 
                WHERE u.username = @author 
                ORDER BY m.pub_date DESC 
                LIMIT @pageSize OFFSET @offset";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.AddWithValue("@author", author);
            cmd.Parameters.AddWithValue("@pageSize", PageSize);
            cmd.Parameters.AddWithValue("@offset", offset);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var authorName = reader.GetString(0);
                var message = reader.GetString(1);
                var timestamp = UnixTimeStampToDateTimeString(reader.GetInt64(2));
                
                cheeps.Add(new CheepViewModel(authorName, message, timestamp));
            }

            return cheeps;
        }

        private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp);
            return dateTime.ToString("MM/dd/yy H:mm:ss");
        }
    }
}
