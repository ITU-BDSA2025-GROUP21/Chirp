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

    public IEnumerable<CheepDTO> GetCheepsFromAuthorEmail(string author, int page = 1)
    {   
        return _cheepRepository.GetByAuthorEmail(author, page, PageSize).Select(createCheepDTO);
    }
    public IEnumerable<CheepDTO> GetCheepsFromMultipleAuthors(List<string> authors, int page = 1)
    {
        return _cheepRepository.GetByMultipleAuthors(authors, page, PageSize).Select(createCheepDTO);
    }

    public void AddCheep(string text, string authorId)
    {
        _cheepRepository.AddCheep(text, authorId);
    }

    public Task DeleteAllCheepsAsync(string id)
    {
        return _cheepRepository.DeleteAllCheepsAsync(id);
    }

    private readonly Func<Cheep, CheepDTO> createCheepDTO =
    c => new CheepDTO
    {
        Author = c.Author.Name,
        Message = c.Text,
        CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
    };
}
