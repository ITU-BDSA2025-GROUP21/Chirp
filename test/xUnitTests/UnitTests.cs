using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
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
            var helgeCheeps = _cheepRepository.GetByAuthorEmail("ropf@itu.dk");

            var adrianCheeps = _cheepRepository.GetByAuthorEmail("adho@itu.dk");

            var nathanCheeps = _cheepRepository.GetByAuthorEmail("Nathan+Sirmon@dtu.dk");

            var johnnieCheeps = _cheepRepository.GetByAuthorEmail("Johnnie+Calixto@itu.dk");

            var jacqualineTwelfthPage = _cheepRepository.GetByAuthorEmail("Jacqualine.Gilcoine@gmail.com", page: 12); // there is 359 entries which means that the 11th page is completely full & and the 12th page has 7 entries

            Assert.Single(helgeCheeps);
            Assert.All(helgeCheeps, c => Assert.Equal("ropf@itu.dk", c.Author.Email));

            Assert.Single(adrianCheeps);
            Assert.All(adrianCheeps, c => Assert.Equal("adho@itu.dk", c.Author.Email));

            Assert.Equal(22, nathanCheeps.Count());
            Assert.All(nathanCheeps, c => Assert.Equal("Nathan+Sirmon@dtu.dk", c.Author.Email));

            Assert.Equal(15, johnnieCheeps.Count());
            Assert.All(johnnieCheeps, c => Assert.Equal("Johnnie+Calixto@itu.dk", c.Author.Email));

            Assert.Equal(7, jacqualineTwelfthPage.Count());
            Assert.All(jacqualineTwelfthPage, c => Assert.Equal("Jacqualine.Gilcoine@gmail.com", c.Author.Email));
        }

        [Fact]
        public void GetCheepsFromAuthor_PaginationWorks()
        {
            var luannaFirstPage = _cheepRepository.GetByAuthorEmail("Luanna-Muro@ku.dk", page: 1);
            var luannaSecondPage = _cheepRepository.GetByAuthorEmail("Luanna-Muro@ku.dk", page: 2);
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

            var testCheep = _testServices._cheepRepository.GetByAuthorEmail("consMail").First();

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

            var cheeps = _cheepRepository.GetByAuthorEmail(author.Email);

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
            Assert.Equal("Helge", AName);
        }

        [Fact]
        public void testAuthorRepositoryGetByEmail()
        {
            var dbContext = _testServices.ctx;
            _authorRepository = _testServices._authorRepository;

            var Author = _authorRepository.FindAuthorByEmail("ropf@itu.dk");
            var AEmail = Author?.Email;
            Assert.NotNull(Author);
            Assert.Equal("ropf@itu.dk", AEmail);
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

            var authorHelge = _authorRepository.FindAuthorByName("Helge");
            var authorAdrian = _authorRepository.FindAuthorByName("Adrian");

            Assert.NotNull(authorHelge);
            Assert.NotNull(authorAdrian);

            Assert.Equal("Helge", authorHelge.Name);
            Assert.Equal("Adrian", authorAdrian.Name);

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

            var authorHelge = _authorRepository.FindAuthorByName("Helge");
            var authorAdrian = _authorRepository.FindAuthorByName("Adrian");

            Assert.NotNull(authorHelge);
            Assert.NotNull(authorAdrian);

            Assert.Equal("Helge", authorHelge.Name);
            Assert.Equal("Adrian", authorAdrian.Name);

            _authorRepository.FollowAuthor(authorHelge, authorAdrian);

            // Follow was not added
            Assert.True(_authorRepository.DoesAuthorFollow(authorHelge, authorAdrian));

            _authorRepository.UnfollowAuthor(authorHelge, authorAdrian);

            // Follow did not get removed.
            Assert.False(_authorRepository.DoesAuthorFollow(authorHelge, authorAdrian));
        }

        [Fact]
        public void testAuthorDeletion()
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

            var testAuthor = _authorRepository.FindAuthorByName("GDPR Test");
            Assert.NotNull(testAuthor);

            _authorRepository.DeleteAuthorByIdAsync(authorToDelete.Id).Wait();
            var deletedAuthor = _authorRepository.FindAuthorByName("GDPR Test");
            Assert.Null(deletedAuthor);
        }

        [Fact]
        public void testCheepDeletion()
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

            var cheepsBeforeDeletion = _cheepRepository.GetByAuthorEmail(author.Email);
            Assert.Single(cheepsBeforeDeletion);

            _cheepRepository.DeleteAllCheepsAsync(author.Id).Wait();
            var cheepsAfterDeletion = _cheepRepository.GetByAuthorEmail(author.Email);
            Assert.Empty(cheepsAfterDeletion);
        }
    }
}