using Chirp.Core.DTO;
using Chirp.Core.Repositories;
using Chirp.Core.Services.Interfaces;

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
        return _cheepRepository.GetAll(page, PageSize);
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int page = 1)
    {
        return _cheepRepository.GetByAuthor(author, page, PageSize);
    }
}
