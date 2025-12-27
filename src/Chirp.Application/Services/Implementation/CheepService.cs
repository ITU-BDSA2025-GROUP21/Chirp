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

        public IEnumerable<CheepDTO> GetCheeps(int page = 1)
        {
            return _cheepRepository.GetAll(page, PageSize).Select(CreateCheepDTO).ToList();
        }

        public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string authorId, int page = 1)
        {
            return _cheepRepository.GetByAuthorId(authorId, page, PageSize).Select(CreateCheepDTO);
        }

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
