using Xunit.Abstractions;
using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Models;
using Chirp.Infrastructure.Data;
using System.Diagnostics;

namespace xUnitTests
{
    public class UnitTests : IClassFixture<TestServices>
    {
        private readonly ITestOutputHelper _output;
        private readonly ICheepRepository _cheepRepository;
        private readonly TestServices _testServices;

        public UnitTests(ITestOutputHelper output, TestServices testService)
        {
            _output = output;
            _cheepRepository = testService.CheepRepository;
        }

        [Fact]
        public void GetCheeps_Pagination_Works()
        {
            int pageSize = 32;
            var firstPage = _cheepRepository.GetAll(page: 1);
            var secondPage = _cheepRepository.GetAll(page: 2);
            Assert.Equal(pageSize, firstPage.Count());
            Assert.Equal(pageSize, secondPage.Count());
            Assert.NotEqual(firstPage.First().Message, secondPage.First().Message);
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

            var controlCheep = new CheepDTO
            {
                Author = "Helge",
                Message = "Hello, BDSA students!",
                CreatedDate = "01/08/2023 12:16"
            };

            Assert.Equal(controlCheep.Author, cheep.Author);
            Assert.Equal(controlCheep.Message, cheep.Message);
            var controlDate = controlCheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            var cheepDate = cheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            Assert.Equal(controlDate, cheepDate);
        }

        [Fact]
        public void testOrder()
        {
            var cheeps = _cheepRepository.GetAll();
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

        public void CreateCheepAndFilterAuthor()
        {
            var Author = new Author()
            {
                Name = "Testing Client",
                Cheeps = new List<Cheep>()
            };

            var Chirp = new Cheep()
            {
                CheepId = 9999,
                AuthorId = "Testing Client",
                Author = Author,
                Text = "Testing.",
                TimeStamp = DateTime.Now
            };

            DbContext dbContext = _testServices.GetDbContext();

            dbContext.Set<Author>().Add(Author);
            dbContext.Set<Cheep>().Add(Chirp);
            dbContext.SaveChanges();

            var cheeps = _cheepRepository.GetByAuthor(Author.Name);

            Assert.Single(cheeps);
            Assert.Equal(Author.Name, cheeps.First().Author);
            Assert.Equal(Chirp.Text, cheeps.First().Message);
        }
    }
}
