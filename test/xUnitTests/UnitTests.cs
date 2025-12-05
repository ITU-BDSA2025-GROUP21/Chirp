using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using Xunit.Abstractions;

namespace xUnitTests
{
    public class UnitTests : IClassFixture<TestServices>
    {
        private readonly ITestOutputHelper _output;
        private readonly ICheepRepository _cheepRepository;
        private readonly TestServices _testServices;
        private IAuthorRepository _authorRepository;
        private UserManager<Author> _userManager;

        public UnitTests(ITestOutputHelper output, TestServices testService)
        {
            _output = output;
            _testServices = testService;
            _cheepRepository = testService._cheepRepository;
            _authorRepository = testService._authorRepository;
            _userManager = testService._userManager;
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
            var helgeCheeps = _cheepRepository.GetByAuthorId("11"); //Change from email

            var adrianCheeps = _cheepRepository.GetByAuthorId("12");

            var nathanCheeps = _cheepRepository.GetByAuthorId("4");

            var johnnieCheeps = _cheepRepository.GetByAuthorId("9");

            var jacqualineTwelfthPage = _cheepRepository.GetByAuthorId("10", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("11", c.AuthorId));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("12", c.AuthorId));

            Assert.Equal(22, nathanCheeps.Count());
            Assert.All(nathanCheeps, c => Assert.Equal("4", c.AuthorId));

            Assert.Equal(15, johnnieCheeps.Count());
            Assert.All(johnnieCheeps, c => Assert.Equal("9", c.AuthorId));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            var luannaFirstPage = _cheepRepository.GetByAuthorId("2", page: 1);
            var luannaSecondPage = _cheepRepository.GetByAuthorId("2", page: 2);
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

            var testCheep = _testServices._cheepRepository.GetByAuthorId("consistency").First();

            Assert.Equal(testCheep.Author.Name, controlCheep.Author.Name);
            Assert.Equal(testCheep.Text, controlCheep.Text);
            var controlDate = controlCheep.TimeStamp.ToString().Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            var cheepDate = cheep.TimeStamp.ToString().Replace("/", "").Replace(" ", "").Replace(".", "").Replace(":", "");
            Assert.Equal(controlDate, cheepDate);
        }

        [Fact]
        public void testOrder()
        {
            var cheeps = _cheepRepository.GetAll();
            DateTime prevTime = DateTime.ParseExact("01/01/3000 00.00", "dd/MM/yyyy HH.mm", CultureInfo.InvariantCulture);
            var ordered = true;

            foreach (Cheep cheep in cheeps)
            {
                var aT = cheep.TimeStamp;

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
                Cheeps = new List<Cheep>(),
                Email = "testEmail@test.test",
                Id = "987654321"
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

            var cheeps = _cheepRepository.GetByAuthorId(author.Id);

            Assert.Single(cheeps);
            Assert.Equal(author.Id, cheeps.First().Author.Id);
            Assert.Equal(Chirp.Text, cheeps.First().Text);
        }

        [Fact]
        public void testAuthorRepositoryGetByName()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var author = _authorRepository.FindAuthorById("11");
            var AId = author?.Id;
            Assert.NotNull(author);
            Assert.Equal("11", AId);
        }

        [Fact]
        public void testAuthorRepositoryGetByEmail()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var Author = _authorRepository.FindAuthorById("11");
            var AId = Author?.Id;
            Assert.NotNull(Author);
            Assert.Equal("11", AId);
        }

        [Fact]
        public void testAuthorRepositoryGetFollowersAndFollowing()
        {
            // Tests the following:
            // - FollowAuthor
            // - GetFollowers
            // - GetFollowing

            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var authorHelge = _authorRepository.FindAuthorById("11");
            var authorAdrian = _authorRepository.FindAuthorById("12");

            Assert.NotNull(authorHelge);
            Assert.NotNull(authorAdrian);

            Assert.Equal("11", authorHelge.Id);
            Assert.Equal("12", authorAdrian.Id);

            Assert.DoesNotContain(authorHelge, _authorRepository.GetFollowers(authorAdrian));
            Assert.DoesNotContain(authorAdrian, _authorRepository.GetFollowing(authorHelge));

            _authorRepository.FollowAuthor(authorHelge, authorAdrian);

            Assert.Contains(authorHelge, _authorRepository.GetFollowers(authorAdrian));
            Assert.Contains(authorAdrian, _authorRepository.GetFollowing(authorHelge));
        }

        [Fact]
        public void testAuthorRepositoryFollowLogic()
        {
            // Tests the following:
            // - FollowAuthor
            // - UnfollowAuthor
            // - DoesAuthorFollow

            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var authorHelge = _authorRepository.FindAuthorById("11");
            var authorAdrian = _authorRepository.FindAuthorById("12");

            Assert.NotNull(authorHelge);
            Assert.NotNull(authorAdrian);

            Assert.Equal("11", authorHelge.Id);
            Assert.Equal("12", authorAdrian.Id);

            _authorRepository.FollowAuthor(authorHelge, authorAdrian);

            // Follow was not added
            Assert.True(_authorRepository.DoesAuthorFollow(authorHelge, authorAdrian));

            _authorRepository.UnfollowAuthor(authorHelge, authorAdrian);

            // Follow did not get removed.
            Assert.False(_authorRepository.DoesAuthorFollow(authorHelge, authorAdrian));
        }

        [Fact]
        public async Task testAuthorDeletion()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            var authorToDelete = new Author()
            {
                Name = "GDPR Test",
                Email = "",
                Cheeps = new List<Cheep>(),
                Id = "GDPR-TEST-ID"
            };

            dbContext.Authors.Add(authorToDelete);
            dbContext.SaveChanges();

            var testAuthor = _authorRepository.FindAuthorById("GDPR-TEST-ID");
            Assert.NotNull(testAuthor);

            await _userManager.DeleteAsync(authorToDelete);

            var deletedAuthor = _authorRepository.FindAuthorById("GDPR-TEST-ID");
            Assert.Null(deletedAuthor);
        }

        [Fact]
        public async Task testCheepDeletion()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;
            var author = new Author()
            {
                Name = "Cheep Deletion Test",
                Email = "",
                Cheeps = new List<Cheep>(),
                Id = "CHEEP-DELETE-TEST-ID"
            };

            var Cheep = new Cheep()
            {
                AuthorId = author.Id,
                Text = "This cheep will be deleted in a test.",
                TimeStamp = DateTime.Now
            };

            dbContext.Authors.Add(author);
            dbContext.Cheeps.Add(Cheep);
            dbContext.SaveChanges();

            var cheepsBeforeDeletion = _cheepRepository.GetByAuthorId(author.Id);
            Assert.Single(cheepsBeforeDeletion);

            await _userManager.DeleteAsync(author);
            var cheepsAfterDeletion = _cheepRepository.GetByAuthorId(author.Id);
            Assert.Empty(cheepsAfterDeletion);
        }

        [Fact]
        public void testLikeFunctionality()
        {
            Author author = new Author()
            {
                Name = "Like Tester",
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

            var cheepFromDb = _cheepRepository.GetByAuthorId(author.Id).First();

            //see that there are no likes initially

            Assert.Equal(0, cheepFromDb.Likes.Count());

            _cheepRepository.Like(cheepFromDb.CheepId, author.Id, true);
            cheepFromDb = _cheepRepository.GetByAuthorId(author.Id).First();

            Assert.Equal(1, cheepFromDb.Likes.Count());
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

            var cheepFromDb = _cheepRepository.GetByAuthorId(author.Id).First();

            //see that there are no dislikes initially

            Assert.Equal(0, cheepFromDb.Likes.Count());

            _cheepRepository.Like(cheepFromDb.CheepId, author.Id, false);
            cheepFromDb = _cheepRepository.GetByAuthorId(author.Id).First();

            int dislikes = cheepFromDb.Likes.Where(l => l.likeStatus == -1).Count();

            Assert.Equal(1, dislikes);
        }
    }

}