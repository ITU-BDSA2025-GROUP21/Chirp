using Chirp.Razor;
using Chirp.Razor.Repositories;
public interface ICheepService
{
    public IEnumerable<CheepDTO> GetCheeps(int page = 1); //Pagination
    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int page = 1); //Pagination
}

public class CheepService : ICheepService
{
    private const int PageSize = 32;

    private readonly ICheepRepository _cheepRepository;

    public CheepService(ICheepRepository cheepRepository) 
    {
        _cheepRepository = cheepRepository;
    }

    public IEnumerable<CheepDTO> GetCheeps(int page = 1) //Pagination 
    {
        return _cheepRepository.GetAll(page, PageSize);
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int page = 1) //Pagination
    {
        return _cheepRepository.GetByAuthor(author, page, PageSize);
    }
}
