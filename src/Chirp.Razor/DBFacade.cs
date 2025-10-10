using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace Chirp.Razor.DBFacade
{
    public class DBFacade
    {
        private readonly string connectionString;
        private const int PageSize = 32;

        public DBFacade()
        {
            string dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH");

            string dataDump = Environment.GetEnvironmentVariable("CHIRPDATADUMP");


            connectionString = initDB(dbPath);

            ImportDataDump(dataDump);
        }

        public string initDB(string dbPath)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "chirp.db");

            if (string.IsNullOrWhiteSpace(dbPath))
                dbPath = tempPath;

            if (!dbPath.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            {
                var directory = Path.GetDirectoryName(dbPath);
                dbPath = string.IsNullOrEmpty(directory) ? tempPath : Path.Combine(directory, "chirp.db");
            }

            string dir = Path.GetDirectoryName(dbPath);

            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string connectionString = $"Data Source={dbPath}";

            string foreignKeyEnforcement = "PRAGMA foreign_keys = ON;";

            string dropTable = @"
                       DROP TABLE IF EXISTS message;
                       DROP TABLE IF EXISTS user;";

            string createUser = @"
                    CREATE TABLE IF NOT EXISTS user (
                    user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT NOT NULL,
                    email TEXT NOT NULL,
                    pw_hash TEXT NOT NULL,
                    UNIQUE (username, email)
                    );";

            string createMessage = @"
                    CREATE TABLE IF NOT EXISTS message (
                    message_id INTEGER PRIMARY KEY AUTOINCREMENT,
                    author_id INTEGER NOT NULL,
                    text TEXT NOT NULL,
                    pub_date INTEGER NOT NULL,
                    UNIQUE (author_id, text, pub_date),
                    FOREIGN KEY (author_id) REFERENCES user(user_id)
                    );";


            ExecuteNonQuery(connectionString, foreignKeyEnforcement, dropTable, createUser, createMessage);

            return connectionString;
        }

        public void ImportDataDump(string dataDump)
        {
            if (string.IsNullOrWhiteSpace(dataDump) || !File.Exists(dataDump))
            {
                return;
            }

            string sql = File.ReadAllText(dataDump);

            ExecuteNonQuery(connectionString, sql);
        }


        public List<CheepViewModel> GetCheeps(int page = 1)
        {
            var offset = (page - 1) * PageSize;

            var query = @"
                SELECT 
                    u.username, 
                    m.text,
                    m.pub_date
                FROM message m 
                JOIN user u 
                    ON m.author_id = u.user_id 
                ORDER BY m.pub_date DESC 
                LIMIT @pageSize OFFSET @offset";

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@pageSize", PageSize);
            parameters.Add("@offset", offset);

            return GetCheepsFromDB(connectionString, query, parameters);

        }

        public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1)
        {
            var cheeps = new List<CheepViewModel>();

            var offset = (page - 1) * PageSize;
            var query = @"
                SELECT 
                    u.username, 
                    m.text, 
                    m.pub_date
                FROM message m 
                JOIN user u 
                    ON m.author_id = u.user_id 
                WHERE u.username = @author 
                ORDER BY m.pub_date DESC 
                LIMIT @pageSize OFFSET @offset";

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@author", author);
            parameters.Add("@pageSize", PageSize);
            parameters.Add("@offset", offset);

            return GetCheepsFromDB(connectionString, query, parameters);
        }

        private List<CheepViewModel> GetCheepsFromDB(string connectionString, string query, Dictionary<string, object> parameters)
        {
            List<CheepViewModel> cheeps = new List<CheepViewModel>();

            using (SqliteDataReader reader = ExecuteReader(connectionString, query, parameters))
            {
                while (reader.Read())
                {
                    var authorName = reader.GetString(0);
                    var message = reader.GetString(1);
                    var timestamp = UnixTimeStampToDateTimeString(reader.GetInt64(2));

                    cheeps.Add(new CheepViewModel(authorName, message, timestamp));
                }
            }

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

            var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

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
