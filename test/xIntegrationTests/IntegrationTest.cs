
using Chirp.Core;
using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Models;
using Xunit.Abstractions;


namespace xIntegrationTests
{

    public class IntegrationTest : IClassFixture<TestServices>
    {

        private readonly ITestOutputHelper _output;
        private readonly ICheepService _cheepService;
        private readonly ICheepRepository _cheepRepository;

        public IntegrationTest(ITestOutputHelper output, TestServices testServices) 
        { 
            _output = output;
            _cheepService = testServices.CheepService;
            _cheepRepository = testServices.CheepRepository;
        }

        [Fact]
        public void CreateCheepAndFilterAuthor()
        {

            var Author = new AuthorDTO()
            {
                Name = "Testing Client",
                Email = "Tester@Cheep.com",
                Password = "TestPassword"
            };

            var Chirp = new CheepDTO()
            {
                Author = Author.Name,
                Message = "Test Message", 
                CreatedDate = "2023-08-01 13:15:37",
            };

            _cheepRepository.CreateNewAuthor(Author.Name, Author.Email, Author.Password);
            _cheepRepository.AddChirp(Chirp);

            var cheeps = _cheepService.GetCheepsFromAuthor(Author.Name);

            Assert.Single(cheeps);
            Assert.Equal(Author.Name, cheeps.First().Author);
            Assert.Equal(Chirp.Message, cheeps.First().Message);
        }
    }
}

