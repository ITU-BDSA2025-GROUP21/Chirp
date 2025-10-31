using Chirp.Core.DTO;
using Chirp.Core.Services;
using Xunit.Abstractions;

namespace XunitTests
{
    public class UnitTest1 : IClassFixture<TestServices>
    {
        private readonly ITestOutputHelper _output;
        private readonly ICheepService _cheepService;


        public UnitTest1(ITestOutputHelper output, TestServices testService)
        {
            _output = output;
            _cheepService = testService.CheepService;
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
            int pageSize = 32;
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

            var controlCheep = new CheepDTO { 
                Author = "Helge",
                Message = "Hello, BDSA students!",
                CreatedDate = "01/08/2023 12:16"
            };

            Assert.Equal(controlCheep, cheep);
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
                } else {
                    ordered = false;
                    break;
                }
            }

            Assert.True(ordered);
        }
    }
}