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

    public void Like(CheepDTO cheepDTO, string authorId)
    {
        _cheepRepository.Like(
            _cheepRepository.GetById(cheepDTO.cheepId)!,
            authorId
            );
    }

    public void Dislike(CheepDTO cheepDTO, string authorId)
    {
        _cheepRepository.Dislike(
            _cheepRepository.GetById(cheepDTO.cheepId)!,
            authorId
            );
    }

    private readonly Func<Cheep, CheepDTO> createCheepDTO =
    c => new CheepDTO
    {
        Author = c.Author.Name,
        Message = c.Text,
        CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm"),
        AuthorId = c.AuthorId,
    };

    
}
