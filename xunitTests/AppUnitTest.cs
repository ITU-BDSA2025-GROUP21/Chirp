using ChirpProject.MainApp;
using ChirpProject.MainApp.CheepClass;
using SimpleDB;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using xunitTests;

namespace MyApp.Tests
{

    public class AppTests
    {
        private readonly App _app;

        
        public AppTests()
        {
            _app = new App(true);

            ((TestDatabase<Cheep>)_app.database).Purge(true);
        }

        [Fact]
        public void SendCheep()
        {
            _app.SendCheep("HelloWorld");

            var cheeps = _app.database.Read().ToList();
            Assert.Contains(cheeps, c => c.Message == "HelloWorld");
        }

        [Fact]
        public void SendCheepWhitespace()
        {
            ((TestDatabase<Cheep>)_app.database).Purge(true);

            _app.SendCheep(" ");
            _app.SendCheep("");
            var cheeps = _app.database.Read().ToList();
            Assert.True(cheeps.Count == 0);
        }

        [Fact]
        public void PurgeCheeps()
        {
            _app.SendCheep("HelloWorld");
            _app.SendCheep("HelloWorld2");
            ((TestDatabase<Cheep>)_app.database).Purge(true);
            var cheeps = _app.database.Read().ToList();
            Assert.True(cheeps.Count == 0);
        }

        [Fact]
        public void ReadCheeps()
        {
            _app.SendCheep("HelloWorld");
            _app.SendCheep("HelloWorld2");
            var cheeps = _app.database.Read().ToList();
            Assert.True(cheeps.Count == 2);
        }
    }
}

