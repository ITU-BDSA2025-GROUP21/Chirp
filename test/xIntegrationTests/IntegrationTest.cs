using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Xunit.Abstractions;
using Chirp.Core.Repositories;

namespace XintegrationTests
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
            var helgeCheeps = _cheepService.GetCheepsFromAuthorEmail("ropf@itu.dk");

            var adrianCheeps = _cheepService.GetCheepsFromAuthorEmail("adho@itu.dk");

            var nathanCheeps = _cheepService.GetCheepsFromAuthorEmail("Nathan+Sirmon@dtu.dk");

            var johnnieCheeps = _cheepService.GetCheepsFromAuthorEmail("Johnnie+Calixto@itu.dk");

            var jacqualineTwelfthPage = _cheepService.GetCheepsFromAuthorEmail("Jacqualine.Gilcoine@gmail.com", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

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
            var luannaFirstPage = _cheepService.GetCheepsFromAuthorEmail("Luanna-Muro@ku.dk", page: 1); //change to email
            var luannaSecondPage = _cheepService.GetCheepsFromAuthorEmail("Luanna-Muro@ku.dko", page: 2);
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
            var controlDate = controlCheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            var cheepDate = cheep.CreatedDate.Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
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
                Email = "testEmail@test.test",
                Cheeps = new List<Cheep>(),
                Id = "1234567789"
            };

            var Chirp = new Cheep()
            {
                AuthorId = author.Id,
                Text = "Test Message",
                TimeStamp = DateTime.Parse("2023-08-01 13:15:37")
            };

            var dbContext = _testServices.ctx;

            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(Chirp);
            dbContext.SaveChanges();

            var cheeps = _cheepService.GetCheepsFromAuthorEmail(author.Email);

            Assert.Single(cheeps); //is empty somehow
            Assert.Equal(author.Name, cheeps.First().Author);
            Assert.Equal(Chirp.Text, cheeps.First().Message);
        }

        [Fact]
        public void testCheepServiceCheepsFromMultipleAuthors()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            _authorService = _testServices._authorService;

            var cheeps = _cheepService.GetCheepsFromMultipleAuthors(new List<string>() { "Helge", "Adrian" }); 
            Assert.All(cheeps, c =>
                Assert.Contains(c.Author, new[] { "Helge", "Adrian" })
            );
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

        [Fact]
        public void testAuthorServiceGetFollowersAndFollowing()
        {
            var dbContext = _testServices.ctx;
            _authorService = _testServices._authorService;

            var helgeDTO = _authorService.FindAuthorByName("Helge");
            var adrianDTO = _authorService.FindAuthorByName("Adrian");

            // make sure both authors exists
            Assert.NotNull(helgeDTO);
            Assert.NotNull(adrianDTO);

            Assert.DoesNotContain(helgeDTO, _authorService.GetFollowers("Adrian"));
            Assert.DoesNotContain(adrianDTO, _authorService.GetFollowing("Helge"));

            _authorService.FollowAuthor("Helge", "Adrian");

            Assert.Contains(helgeDTO, _authorService.GetFollowers("Adrian"));
            Assert.Contains(adrianDTO, _authorService.GetFollowing("Helge"));
        }

        [Fact]
        public void testAuthorServiceFollowLogic()
        {
            var dbContext = _testServices.ctx;
            _authorService = _testServices._authorService;

            var helgeDTO = _authorService.FindAuthorByName("Helge");
            var adrianDTO = _authorService.FindAuthorByName("Adrian");

            // make sure both authors exists
            Assert.NotNull(helgeDTO);
            Assert.NotNull(adrianDTO);

            _authorService.FollowAuthor("Helge", "Adrian");

            // Follow was not added
            Assert.True(_authorService.IsFollowing("Helge", "Adrian"));

            _authorService.UnfollowAuthor("Helge", "Adrian");

            // Follow did not get removed.
            Assert.False(_authorService.IsFollowing("Helge", "Adrian"));
        }

        [Fact]
        public void testAuthorDeletion()
        {
            var author = new Author()
            {
                Name = "Delete Test",
                Email = "delMail",
                Cheeps = new List<Cheep>(),
                Id = "DeleteID",
            };

            var dbContext = _testServices.ctx;
            dbContext.Authors.Add(author);
            dbContext.SaveChanges();

            //see that author exists
            var authorDTO = _authorService.FindAuthorByName("Delete Test");
            Assert.NotNull(authorDTO);

            //delete author and see that it is deleted
            _authorService.DeleteAuthorByIdAsync("DeleteID").Wait();
            var deletedAuthorDTO = _authorService.FindAuthorByName("Delete Test");
            Assert.Null(deletedAuthorDTO);
        }
    }
}