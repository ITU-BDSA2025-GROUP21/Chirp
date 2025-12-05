using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using System.Linq.Expressions;
using NuGet.Protocol.Core.Types;

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
        return _cheepRepository.GetAll(page, PageSize).Select(createCheepDTO).ToList();
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthorId(string authorId, int page = 1)
    {   
        return _cheepRepository.GetByAuthorId(authorId, page, PageSize).Select(createCheepDTO);
    }
    public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authorIds, int page = 1)
    {
        return _cheepRepository.GetByMultipleAuthors(authorIds, page, PageSize).Select(createCheepDTO);
    }

    public void AddCheep(string text, string authorId)
    {
        _cheepRepository.AddCheep(text, authorId);
    }

    public Task DeleteAllCheepsAsync(string id)
    {
        return _cheepRepository.DeleteAllCheepsAsync(id);
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
        return _cheepRepository.GetById(cheepId) is Cheep cheep ? createCheepDTO(cheep) : null;
    }

    private readonly Func<Cheep, CheepDTO> createCheepDTO =
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

    public Likes getLike(int cheepId, string authorId, bool like)
    {
        return _cheepRepository.getLike(cheepId, authorId, like);
    }
}
