using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using System.Linq.Expressions;

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
        return _cheepRepository.GetAll(page, PageSize).Select(createCheepDTO);
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int page = 1)
    {   
        return _cheepRepository.GetByAuthor(author, page, PageSize).Select(createCheepDTO);
    }

    private readonly Func<Cheep, CheepDTO> createCheepDTO =
    c => new CheepDTO
    {
        Author = c.Author.Name,
        Message = c.Text,
        CreatedDate = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
    };
}
