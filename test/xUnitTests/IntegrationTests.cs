using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Chirp.Core.Repositories;
using Chirp.Razor.Repositories;
using Chirp.Infrastructure.Services;

namespace XunitTests
{
    public class IntegrationTests : IClassFixture<TestServices>
    {
        private readonly ITestOutputHelper _output;
        private readonly ICheepService _cheepService;
        private readonly TestServices _testServices;
        private IAuthorService _authorService;
        private IAuthorRepository _authorRepository;

        public IntegrationTests(ITestOutputHelper output, TestServices testService)
        {
            _output = output;
            _testServices = testService;
            _cheepService = testService._cheepService;
            _authorService = testService._authorService;
        }

        [Fact]
        public void GetCheeps_Pagination_Works()
        {
            int pageSize = 32;
            var firstPage = _cheepService.GetCheeps(page: 1);
            var secondPage = _cheepService.GetCheeps(page: 2);
            Assert.Equal(pageSize, firstPage.Count());
            Assert.Equal(pageSize, secondPage.Count());
            Assert.NotEqual(firstPage.First().Message, secondPage.First().Message);
        }

        [Fact]
        public void GetCheepsFromAuthor_FilteringWorks()
        {
            var helgeCheeps = _cheepService.GetCheepsFromAuthor("Helge");

            var adrianCheeps = _cheepService.GetCheepsFromAuthor("Adrian");

            var nathanCheeps = _cheepService.GetCheepsFromAuthor("Nathan Sirmon");

            var johnnieCheeps = _cheepService.GetCheepsFromAuthor("Johnnie Calixto");

            var jacqualineTwelfthPage = _cheepService.GetCheepsFromAuthor("Jacqualine Gilcoine", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("Helge", c.Author));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("Adrian", c.Author));

            Assert.Equal(22, nathanCheeps.Count());
            Assert.All(nathanCheeps, c => Assert.Equal("Nathan Sirmon", c.Author));

            Assert.Equal(15, johnnieCheeps.Count());
            Assert.All(johnnieCheeps, c => Assert.Equal("Johnnie Calixto", c.Author));

            Assert.Equal(7, jacqualineTwelfthPage.Count());
            Assert.All(jacqualineTwelfthPage, c => Assert.Equal("Jacqualine Gilcoine", c.Author));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            var luannaFirstPage = _cheepService.GetCheepsFromAuthor("Luanna Muro", page: 1);
            var luannaSecondPage = _cheepService.GetCheepsFromAuthor("Luanna Muro", page: 2);
            Assert.NotEmpty(luannaFirstPage);
            Assert.Empty(luannaSecondPage);
        }

        [Fact]
        public void testConsistency()
        {
            var cheeps = _cheepService.GetCheeps();
            var cheep = cheeps.First();

            var controlCheep = new CheepDTO
            {
                Author = "Helge",
                Message = "Hello, BDSA students!",
                CreatedDate = "01/08/2023 12.16"
            };

            Assert.Equal(controlCheep.Author, cheep.Author);
            Assert.Equal(controlCheep.Message, cheep.Message);
            var controlDate = controlCheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "").Replace("-", "");
            var cheepDate = cheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "").Replace("-", "");
            Assert.Equal(controlDate, cheepDate);
        }

        [Fact]
        public void testOrder()
        {
            var cheeps = _cheepService.GetCheeps();
            DateTime prevTime = DateTime.Parse("01/01/00 00:00");
            var ordered = true;

            foreach (CheepDTO cheep in cheeps)
            {
                var time = cheep.CreatedDate;
                DateTime aT = DateTime.Parse(time);

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

            var cheeps = _cheepService.GetCheepsFromAuthor(author.Name);

            Assert.Single(cheeps);
            Assert.Equal(author.Name, cheeps.First().Author);
            Assert.Equal(Chirp.Text, cheeps.First().Message);
        }

        [Fact]
        public void testAuthorServiceFindByName()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            _authorService = _testServices._authorService;

            var Author = _authorService.FindAuthorByName("Helge");
            var AName = Author?.Name;
            Assert.NotNull(Author);
            Assert.Equal(AName, "Helge");
        }

        [Fact]
        public void testAuthorServiceFindByEmail()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            _authorService = _testServices._authorService;

            var Author = _authorService.FindAuthorByEmail("ropf@itu.dk");
            var AEmail = Author?.Email;
            Assert.NotNull(Author);
            Assert.Equal(AEmail, "ropf@itu.dk");
        }
    }
}