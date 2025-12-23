using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Razor.Repositories;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Diagnostics;
using System.Globalization;
using Xunit.Abstractions;

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
            var helgeCheeps = _cheepService.GetCheepsFromAuthorId("11"); //Change from email

            var adrianCheeps = _cheepService.GetCheepsFromAuthorId("12");

            var nathanCheeps = _cheepService.GetCheepsFromAuthorId("4");

            var johnnieCheeps = _cheepService.GetCheepsFromAuthorId("9");

            var jacqualineTwelfthPage = _cheepService.GetCheepsFromAuthorId("10", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("11", c.AuthorId));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("12", c.AuthorId));

            Assert.Equal(22, nathanCheeps.Count());
            Assert.All(nathanCheeps, c => Assert.Equal("4", c.AuthorId));

            Assert.Equal(15, johnnieCheeps.Count());
            Assert.All(johnnieCheeps, c => Assert.Equal("9", c.AuthorId));

            Assert.Equal(7, jacqualineTwelfthPage.Count());
            Assert.All(jacqualineTwelfthPage, c => Assert.Equal("10", c.AuthorId));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            var luannaFirstPage = _cheepService.GetCheepsFromAuthorId("2", page: 1);
            var luannaSecondPage = _cheepService.GetCheepsFromAuthorId("2", page: 2);
            Assert.NotEmpty(luannaFirstPage);
            Assert.Empty(luannaSecondPage);
        }

        [Fact]
        public void testConsistency()
        {
            var author = new Author()
            {
                Name = "ConsistencyAuthor",
                Email = "consMail",
                Cheeps = new List<Cheep>(),
                Id = "consistency",
            };

            var cheep = new Cheep()
            {
                AuthorId = author.Id,
                Text = "Consistent Message",
                TimeStamp = DateTime.Parse("2023-08-01 14:15:37")
            };

            var dbContext = _testServices.ctx;
            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(cheep);
            dbContext.SaveChanges();

            var controlCheep = new Cheep
            {
                Author = author,
                Text = "Consistent Message",
                TimeStamp = DateTime.Parse("2023-08-01 14:15:37")
            };

            var testCheep = _testServices._cheepService.GetCheepsFromAuthorId("consistency").First();

            Assert.Equal(testCheep.Author, controlCheep.Author.Name);
            Assert.Equal(testCheep.Message, controlCheep.Text);
            var controlDate = controlCheep.TimeStamp.ToString().Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            var cheepDate = cheep.TimeStamp.ToString().Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            Assert.Equal(controlDate, cheepDate);
        }

        [Fact]
        public void testOrder()
        {
            var cheeps = _cheepService.GetCheeps();
            DateTime prevTime = DateTime.MaxValue;
            var ordered = true;

            var formats = new[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH.mm", "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH.mm" };

            foreach (CheepDTO cheep in cheeps)
            {
                var time = cheep.CreatedDate;
                DateTime aT = DateTime.ParseExact(time, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                if (aT <= prevTime)
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

            var cheeps = _cheepService.GetCheepsFromAuthorId("1234567789");

            Assert.Single(cheeps); //is empty somehow
            Assert.Equal(author.Id, cheeps.First().AuthorId);
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
        public void testAuthorServiceFindById()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            _authorService = _testServices._authorService;

            var Author = _authorService.FindAuthorById("10");
            var Aid = Author?.Id;
            Assert.NotNull(Author);
            Assert.Equal(Aid, "10");
        }

        [Fact]
        public void testAuthorServiceGetFollowersAndFollowing()
        {
            var dbContext = _testServices.ctx;
            _authorService = _testServices._authorService;

            var helgeDTO = _authorService.FindAuthorById("11");
            var adrianDTO = _authorService.FindAuthorById("12");

            // make sure both authors exists
            Assert.NotNull(helgeDTO);
            Assert.NotNull(adrianDTO);

            Assert.DoesNotContain(helgeDTO, _authorService.GetFollowers("12"));
            Assert.DoesNotContain(adrianDTO, _authorService.GetFollowing("11"));

            _authorService.FollowAuthor("11", "12");

            Assert.Contains(helgeDTO, _authorService.GetFollowers("12"));
            Assert.Contains(adrianDTO, _authorService.GetFollowing("11"));
        }

        [Fact]
        public void testAuthorServiceFollowLogic()
        {
            var dbContext = _testServices.ctx;
            _authorService = _testServices._authorService;

            var helgeDTO = _authorService.FindAuthorById("11");
            var adrianDTO = _authorService.FindAuthorById("12");

            // make sure both authors exists
            Assert.NotNull(helgeDTO);
            Assert.NotNull(adrianDTO);

            _authorService.FollowAuthor("11", "12");

            // Follow was not added
            Assert.True(_authorService.IsFollowing("11", "12"));

            _authorService.UnfollowAuthor("11", "12");

            // Follow did not get removed.
            Assert.False(_authorService.IsFollowing("11", "12"));
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
            var authorDTO = _authorService.FindAuthorById("DeleteID");
            Assert.NotNull(authorDTO);

            //delete author and see that it is deleted
            _authorService.DeleteAuthorByIdAsync("DeleteID").Wait();
            var deletedAuthorDTO = _authorService.FindAuthorById("DeleteID");
            Assert.Null(deletedAuthorDTO);
        }

        [Fact]
        public void testCheepDeletion()
        {
            var author = new Author()
            {
                Name = "Cheep Delete Test",
                Email = "cheepDelMail",
                Cheeps = new List<Cheep>(),
                Id = "CheepDeleteID",
            };

            var cheep = new Cheep()
            {
                AuthorId = author.Id,
                Text = "Cheep Delete Test Message",
                TimeStamp = DateTime.Parse("2023-08-01 14:15:37")
            };

            var dbContext = _testServices.ctx;

            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(cheep);
            dbContext.SaveChanges();

            //see that cheep exists
            var cheeps = _cheepService.GetCheepsFromAuthorId("CheepDeleteID");
            Assert.Single(cheeps);

            //delete cheeps and see that they are deleted
            _cheepService.DeleteAllCheepsAsync("CheepDeleteID").Wait();
            var deletedCheeps = _cheepService.GetCheepsFromAuthorId("CheepDeleteID");
            Assert.Empty(deletedCheeps);
        }

        [Fact]
        public void testLikeFunctionality()
        {
            Author author = new Author()
            {
                Name = "like Tester",
                Email = "like@test.com"
            };

            var cheep = new Cheep()
            {
                Author = author,
                Text = "This cheep is for like testing.",
                TimeStamp = DateTime.Now
            };

            var dbContext = _testServices.ctx;
            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(cheep);
            dbContext.SaveChanges();

            var cheepFromDb = _cheepService.GetCheepsFromAuthorId(author.Id).First();

            //see that there are no likes initially

            Assert.Equal(0, cheepFromDb.Likes);

            _cheepService.Like(cheepFromDb.cheepId, author.Id, true);
            cheepFromDb = _cheepService.GetCheepsFromAuthorId(author.Id).First();

            Assert.Equal(1, cheepFromDb.Likes);
        }

        [Fact]
        public void testDislikeFunctionality()
        {
            Author author = new Author()
            {
                Name = "dislike Tester",
                Email = "dislike@test.com"
            };

            var cheep = new Cheep()
            {
                Author = author,
                Text = "This cheep is for dislike testing.",
                TimeStamp = DateTime.Now
            };

            var dbContext = _testServices.ctx;
            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(cheep);
            dbContext.SaveChanges();

            var cheepFromDb = _cheepService.GetCheepsFromAuthorId(author.Id).First();

            //see that there are no dislikes initially

            Assert.Equal(0, cheepFromDb.Dislikes);

            _cheepService.Like(cheepFromDb.cheepId, author.Id, false);
            cheepFromDb = _cheepService.GetCheepsFromAuthorId(author.Id).First();

            Assert.Equal(1, cheepFromDb.Dislikes);
        }

        [Fact]
        public void testKarma()
        {
            Author author = new Author()
            {
                Name = "Karma Tester",
                Email = ""
            };

            var dbContext = _testServices.ctx;
            dbContext.Authors.Add(author);
            dbContext.SaveChanges();

            Assert.Equal(0, _authorService.GetKarmaScore(author.Id));

            _authorService.changeKarma(5, author.Id);

            Assert.Equal(5, _authorService.GetKarmaScore(author.Id));
        }
    }
}