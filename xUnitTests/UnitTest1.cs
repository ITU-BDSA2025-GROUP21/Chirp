using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using Microsoft.Data.Sqlite;
using Chirp.Razor.DBFacade;
using static System.Environment;

namespace XunitTests
{
    public class UnitTest1
    {
        private readonly string dbPath;
        private readonly string connectionString;

        public UnitTest1()
        {
            // unique file per test class instance
            dbPath = Path.Combine(Path.GetTempPath(), $"chirp_{Guid.NewGuid():N}.db");

            // tells app/DBFacade where to create/open the DB
            Environment.SetEnvironmentVariable("CHIRPDBPATH", dbPath);

            connectionString = $"Data Source={dbPath}";
        }


        private void SeedTestData()
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO user (username, email, pw_hash)
                VALUES ('alice', 'alice@mail.com', 'pw'),
                       ('bob', 'bob@mail.com', 'pw');

                INSERT INTO message (author_id, text, pub_date)
                VALUES 
                    (1, 'hello world', strftime('%s','2024-01-01')),
                    (2, 'hey from bob', strftime('%s','2024-01-02')),
                    (1, 'second message', strftime('%s','2024-01-03'));
            ";
            cmd.ExecuteNonQuery();
        }

        private void SeedTestData(int messageCount)
        {
            using var conn = new SqliteConnection(connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();

            // Insert fixed users first
            cmd.CommandText = @"
                INSERT INTO user (username, email, pw_hash)
                VALUES ('alice', 'alice@mail.com', 'pw'),
                       ('bob', 'bob@mail.com', 'pw');
            ";
            cmd.ExecuteNonQuery();

            // Insert variable number of messages
            for (int i = 1; i <= messageCount; i++)
            {
                var authorId = (i % 2) + 1; // alternate between user 1 and 2
                cmd.CommandText = $@"
                    INSERT INTO message (author_id, text, pub_date)
                    VALUES ({authorId}, 'test message #{i}', strftime('%s','now'));
                ";
                cmd.ExecuteNonQuery();
            }
        }


        [Fact]
        public void GetCheeps_OrderedDescending()
        {
            var db = new DBFacade();
            db.initDB();
            SeedTestData();

            var cheeps = db.GetCheeps();

            Assert.NotEmpty(cheeps);
            Assert.Equal(3, cheeps.Count);
            Assert.Equal("alice", cheeps[0].Author);
            Assert.Equal("second message", cheeps[0].Message);
            Assert.Equal("bob", cheeps[1].Author);
        }

        [Fact]
        public void GetCheeps_ReturnRightAmount()
        {
            int amount = 5;
            var db = new DBFacade();
            db.initDB();
            SeedTestData(amount);

            var cheeps = db.GetCheeps();

            Assert.Equal(amount, cheeps.Count);
        }

        [Fact]
        public void GetCheeps_Pagination_Works()
        {
            int totalMessages = 64;
            int pageSize = 32;
            var db = new DBFacade();
            db.initDB();
            SeedTestData(totalMessages);
            var firstPage = db.GetCheeps(page: 1);
            var secondPage = db.GetCheeps(page: 2);
            Assert.Equal(pageSize, firstPage.Count);
            Assert.Equal(pageSize, secondPage.Count);
            Assert.NotEqual(firstPage[0].Message, secondPage[0].Message);
        }

        [Fact]
        public void GetCheepsFromAuthor_FilteringWorks()
        {
            var db = new DBFacade();
            db.initDB();
            SeedTestData();
            var aliceCheeps = db.GetCheepsFromAuthor("alice");
            var bobCheeps = db.GetCheepsFromAuthor("bob");
            Assert.Equal(2, aliceCheeps.Count);
            Assert.All(aliceCheeps, c => Assert.Equal("alice", c.Author));
            Assert.Single(bobCheeps);
            Assert.All(bobCheeps, c => Assert.Equal("bob", c.Author));
        }

        [Fact]
        public void GetCheepsFromAuthor_decendingWorks()
        {
            var db = new DBFacade();
            db.initDB();
            SeedTestData();
            var aliceCheeps = db.GetCheepsFromAuthor("alice");
            Assert.Equal("second message", aliceCheeps[0].Message);
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            int totalMessages = 64;
            int pageSize = 32;
            var db = new DBFacade();
            db.initDB();
            SeedTestData(totalMessages);
            var aliceFirstPage = db.GetCheepsFromAuthor("alice", page: 1);
            var aliceSecondPage = db.GetCheepsFromAuthor("alice", page: 2);
            Assert.Equal(pageSize, aliceFirstPage.Count); // half messages by alice
            Assert.Empty(aliceSecondPage); // only 32 messages total, so page 2 should be empty
        }

        [Fact]
        public void getCheeps_from_empty_database()
        {
            var db = new DBFacade();
            db.initDB();
            var cheeps = db.GetCheeps();
            Assert.Empty(cheeps);
        }
    }
}