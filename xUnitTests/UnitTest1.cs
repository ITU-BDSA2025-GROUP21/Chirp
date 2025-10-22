using Chirp.Razor.DBFacade;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Xunit.Abstractions;
using Chirp.Razor.Data;
using static System.Environment;
using System.Runtime.Serialization.Formatters;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace XunitTests
{
    public class UnitTest1 : IDisposable
    {
        private readonly string dbPath;
        private readonly string connectionString;
        private readonly ITestOutputHelper _output;
        private readonly DBFacade db;


        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;

            // unique file per test class instance
            dbPath = Path.Combine(Path.GetTempPath(), $"chirp_{Guid.NewGuid():N}.db");

            // tells app/DBFacade where to create/open the DB
            Environment.SetEnvironmentVariable("CHIRPDBPATH", dbPath);

            connectionString = $"Data Source={dbPath}";

            db = new DBFacade();
        }

        [Fact]
        public void GetCheeps_Pagination_Works()
        {
            int pageSize = 32;
            var firstPage = db.GetCheepPage(page: 1);
            var secondPage = db.GetCheepPage(page: 2);
            Assert.Equal(pageSize, firstPage.Count);
            Assert.Equal(pageSize, secondPage.Count);
            Assert.NotEqual(firstPage[0].Message, secondPage[0].Message);
        }


        [Fact]
        public void GetCheepsFromAuthor_FilteringWorks()
        {
            var helgeCheeps = db.GetCheepsFromAuthor("Helge");

            var adrianCheeps = db.GetCheepsFromAuthor("Adrian");

            var nathanCheeps = db.GetCheepsFromAuthor("Nathan Sirmon");

            var johnnieCheeps = db.GetCheepsFromAuthor("Johnnie Calixto");

            var jacqualineTwelfthPage = db.GetCheepsFromAuthor("Jacqualine Gilcoine", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("Helge", c.Author));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("Adrian", c.Author));

            Assert.Equal(22, nathanCheeps.Count);
            Assert.All(nathanCheeps, c => Assert.Equal("Nathan Sirmon", c.Author));

            Assert.Equal(15, johnnieCheeps.Count);
            Assert.All(johnnieCheeps, c => Assert.Equal("Johnnie Calixto", c.Author));

            Assert.Equal(7, jacqualineTwelfthPage.Count);
            Assert.All(jacqualineTwelfthPage, c => Assert.Equal("Jacqualine Gilcoine", c.Author));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            int pageSize = 32;
            var luannaFirstPage = db.GetCheepsFromAuthor("Luanna Muro", page: 1);
            var luannaSecondPage = db.GetCheepsFromAuthor("Luanna Muro", page: 2);
            Assert.NotEmpty(luannaFirstPage);
            Assert.Empty(luannaSecondPage);
        }

        [Fact]
        public void testConsistency()
        {
            var cheeps = db.GetCheeps();
            var cheep = cheeps[0];

            var controlCheep = new CheepViewModel(
                "Helge",
                "Hello, BDSA students!",
                "08/01/23 12.16.48"
                );

            Assert.Equal(controlCheep, cheep);
        }

        [Fact]
        public void testOrder()
        { 
            var cheeps = db.GetCheeps();
            DateTime prevTime = DateTime.Parse("01/01/00 00.00.00");
            var ordered = true;

            foreach (CheepViewModel cheep in cheeps)
            {
                var time = cheep.Timestamp;
                DateTime aT = DateTime.Parse(time);

                if (aT >= prevTime)
                {
                    prevTime = aT;
                } else {
                    ordered = false;
                    break;
                }
            }

            Assert.True(ordered);
        }


        public void Dispose()
        {
            db.resetDB();
        }
    }
}