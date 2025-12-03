using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Xunit.Abstractions;

namespace xUnitTests
{
    public class UnitTests : IClassFixture<TestServices>
    {
        private readonly ITestOutputHelper _output;
        private readonly ICheepRepository _cheepRepository;
        private readonly TestServices _testServices;
        private IAuthorRepository _authorRepository;

        public UnitTests(ITestOutputHelper output, TestServices testService)
        {
            _output = output;
            _cheepRepository = testService._cheepRepository;
            _testServices = testService;
        }

        [Fact]
        public void GetCheeps_Pagination_Works()
        {
            int pageSize = 32;
            var firstPage = _cheepRepository.GetAll(page: 1);
            var secondPage = _cheepRepository.GetAll(page: 2);
            Assert.Equal(pageSize, firstPage.Count());
            Assert.Equal(pageSize, secondPage.Count());
            Assert.NotEqual(firstPage.First().Text, secondPage.First().Text);
        }

        [Fact]
        public void GetCheepsFromAuthor_FilteringWorks()
        {
            var helgeCheeps = _cheepRepository.GetByAuthor("Helge");

            var adrianCheeps = _cheepRepository.GetByAuthor("Adrian");

            var nathanCheeps = _cheepRepository.GetByAuthor("Nathan Sirmon");

            var johnnieCheeps = _cheepRepository.GetByAuthor("Johnnie Calixto");

            var jacqualineTwelfthPage = _cheepRepository.GetByAuthor("Jacqualine Gilcoine", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("Helge", c.Author.Name));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("Adrian", c.Author.Name));

            Assert.Equal(22, nathanCheeps.Count());
            Assert.All(nathanCheeps, c => Assert.Equal("Nathan Sirmon", c.Author.Name));

            Assert.Equal(15, johnnieCheeps.Count());
            Assert.All(johnnieCheeps, c => Assert.Equal("Johnnie Calixto", c.Author.Name));

            Assert.Equal(7, jacqualineTwelfthPage.Count());
            Assert.All(jacqualineTwelfthPage, c => Assert.Equal("Jacqualine Gilcoine", c.Author.Name));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            var luannaFirstPage = _cheepRepository.GetByAuthor("Luanna Muro", page: 1);
            var luannaSecondPage = _cheepRepository.GetByAuthor("Luanna Muro", page: 2);
            Assert.NotEmpty(luannaFirstPage);
            Assert.Empty(luannaSecondPage);
        }

        [Fact]
        public void testConsistency()
        {
            var cheeps = _cheepRepository.GetAll();
            var cheep = cheeps.First();

            var controlCheep = new Cheep
            {
                CheepId = 999999,
                AuthorId = "Helge",
                Author = new Author { Name = "Helge" },
                Text = "Hello, BDSA students!",
                TimeStamp = DateTime.Parse("01/08/2023 12:16:48")
            };

            Assert.Equal(controlCheep.AuthorId, cheep.Author.Name);
            Assert.Equal(controlCheep.Text, cheep.Text);
            var controlDate = controlCheep.TimeStamp.ToString("ddMMyyyyHHmmss").Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            Console.WriteLine("Control Date: " + controlDate);
            var cheepDate = cheep.TimeStamp.ToString().Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            Assert.Equal(controlDate, cheepDate);
        }

        [Fact]
        public void testOrder()
        {
            var cheeps = _cheepRepository.GetAll();
            DateTime prevTime = DateTime.Parse("01/01/00 00:00");
            var ordered = true;

            foreach (Cheep cheep in cheeps)
            {
                var aT = cheep.TimeStamp;

                if (aT >= prevTime)
                {
                    prevTime = aT;
                }
                else
                {
                    ordered = false;
                    break;
                }
            }

            Assert.True(ordered);
        }

        [Fact]
        public void CreateCheepAndFilterAuthor()
        {
            var author = new Author()
            {
                Name = "Testing Client",
                Cheeps = new List<Cheep>()
            };

            var Chirp = new Cheep()
            {
                Author = author,
                Text = "Test Message",
                TimeStamp = DateTime.Parse("2023-08-01 13:15:37")
            };

            var dbContext = _testServices.ctx;

            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(Chirp);
            dbContext.SaveChanges();

            var cheeps = _cheepRepository.GetByAuthor(author.Name);

            Assert.Single(cheeps);
            Assert.Equal(author.Name, cheeps.First().Author.Name);
            Assert.Equal(Chirp.Text, cheeps.First().Text);
        }

        [Fact]
        public void testAuthorRepositoryGetByName()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var author = _authorRepository.FindAuthorByName("Helge");
            var AName = author?.Name;
            Assert.NotNull(author);
            Assert.Equal(AName, "Helge");
        }

        [Fact]
        public void testAuthorRepositoryGetByEmail()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var Author = _authorRepository.FindAuthorByEmail("ropf@itu.dk");
            var AEmail = Author?.Email;
            Assert.NotNull(Author);
            Assert.Equal(AEmail, "ropf@itu.dk");
        }
    }
}
