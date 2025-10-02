using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Chirp.Razor.DBFacade
{
    public class DBFacade
    {
        private readonly string chirpDBpath = "Data Source=chirp.db";

        public DBFacade()
        {
            initDB();
        }

        public void initDB()
        {
            using (var conn = new SqliteConnection(chirpDBpath))
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

        public void insertDataDump()
        {
            var dumpPath = Console.ReadLine(); //input

            if (!File.Exists(dumpPath))
            {
                Console.WriteLine($"File '{dumpPath}' does not exist.");
                return;
            }

            string sql = File.ReadAllText(dumpPath);

            using var conn = new SqliteConnection(chirpDBpath);
            conn.Open();

            using var trans = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            cmd.CommandText = sql;

            try
            {
                cmd.ExecuteNonQuery();
                trans.Commit();
                Console.WriteLine("Dump imported with success....");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failure under import: {ex.Message}");
                trans.Rollback();
            }
        }

        public void processCommand(string s) {
        
            switch (s) 
            {
                case "cheep":
                    Console.WriteLine("Write your cheep now!");
                    handleCheep();
                    break;

                case "read":
                    Console.WriteLine("Reading cheeps");
                    readCheeps();
                    break;

                case "listUsers":
                    Console.WriteLine("Listing users");
                    listUsers();
                    break;

                case "exit":
                    Console.WriteLine("Bye bye. T_T");
                    break;

                case "import":
                    Console.WriteLine("Write: <path-to-sql-file>");
                    insertDataDump();
                    break;

                default:
                    Console.WriteLine("Not a valid command");
                    break;
            }
        }

        private void handleCheep()
        {
            return;

        }

        private void readCheeps()
        {
            using var conn = new SqliteConnection(chirpDBpath);
            conn.Open();

            string selectMessages = "SELECT author_id, text, pub_date FROM message;";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = selectMessages;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {

            }
        }

        private void listUsers()
        {
            using var conn = new SqliteConnection(chirpDBpath);
            conn.Open();

            string selectUsers = "SELECT * FROM user;";

            using var cmd = conn.CreateCommand();
            cmd.CommandText = selectUsers;

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader.GetInt32(0)}, Name: {reader.GetString(1)}, Email: {reader.GetString(2)}, Hash: {reader.GetString(3)}");
            }
        }
    }
}
