using Chirp.Application.DTO;
using Chirp.Application.Services.Interface;
using Chirp.Core.Models;
using Chirp.Core.Repositories;

namespace Chirp.Application.Services.Implementation
{
    public class CheepService : ICheepService
    {
        private const int PageSize = 32;

        private readonly ICheepRepository _cheepRepository;

        public CheepService(ICheepRepository cheepRepository)
        {
            _cheepRepository = cheepRepository;
        }

        /// <summary>
        /// Retrieves a paginated collection of cheeps for the specified page.
        /// </summary>
        /// <remarks>Use this method to access cheeps in a paginated manner. The number of cheeps per page
        /// is determined by the PageSize setting.</remarks>
        /// <param name="page">The page number to retrieve. Must be greater than or equal to 1. The first page is 1.</param>
        /// <returns>An <see cref="IEnumerable{CheepDTO}"/> containing cheeps for the specified page. Returns an empty collection
        /// if no cheeps are available for the page.</returns>
        public IEnumerable<CheepDTO> GetCheeps(int page = 1)
        {
            return _cheepRepository.GetAll(page, PageSize).Select(CreateCheepDTO).ToList();
        }

        /// <summary>
        /// Retrieves a paginated collection of cheeps authored by the specified user.
        /// </summary>
        /// <param name="authorId">The unique identifier of the author whose cheeps are to be retrieved. Cannot be null or empty.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1. Defaults to 1.</param>
        /// <returns>An enumerable collection of <see cref="CheepDTO"/> objects representing the cheeps authored by the specified user.
        /// Returns an empty collection if no cheeps are found for the given author or page.</returns>
        public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string authorId, int page = 1)
        {
            return _cheepRepository.GetByAuthorId(authorId, page, PageSize).Select(CreateCheepDTO);
        }

        /// <summary>
        /// Retrieves a paginated collection of cheeps posted by the specified authors.
        /// </summary>
        /// <param name="authorIds">A list of author identifiers. Only cheeps created by these authors will be included in the result.</param>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1. The default value is 1.</param>
        /// <returns>An enumerable collection of <see cref="CheepDTO"/> objects representing cheeps from the specified authors
        /// for the requested page. If no cheeps are found, the collection will be empty.</returns>
        public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authorIds, int page = 1)
        {
            return _cheepRepository.GetByMultipleAuthors(authorIds, page, PageSize).Select(CreateCheepDTO);
        }

        public void AddCheep(string text, string authorId)
        {
            _cheepRepository.AddCheep(text, authorId);
        }

        public void Like(int cheepId, string authorId, bool like)
        {
            _cheepRepository.Like(
                cheepId,
                authorId,
                like
            );
        }

        public CheepDTO? GetById(int cheepId)
        {
            return _cheepRepository.GetById(cheepId) is Cheep cheep ? CreateCheepDTO(cheep) : null;
        }

        private readonly Func<Cheep, CheepDTO> CreateCheepDTO =
            c => new CheepDTO
            {
                Author = c.Author.Name,
                Message = c.Text,
                CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm"),
                AuthorId = c.AuthorId,
                cheepId = c.CheepId,
                Likes = c.Likes.Count(l => l.likeStatus == 1),
                Dislikes = c.Likes.Count(l => l.likeStatus == -1),
            };

        public Task<Like> GetLikeAsync(int cheepId, string authorId, bool like)
        {
            return _cheepRepository.GetLikeAsync(cheepId, authorId, like);
        }
    }
}
