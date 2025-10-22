using Chirp.Razor.DBFacade;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page = 1); //Pagination
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1); //Pagination
}

public class CheepService : ICheepService
{
    private readonly DBFacade _dbFacade; //Database-Dependency

    public CheepService(DBFacade dbFacade) //Constructor til Database-Dependency
    {
        _dbFacade = dbFacade;
    }

    public List<CheepViewModel> GetCheeps(int page = 1) //Pagination 
    {
        return _dbFacade.GetCheepPage(page); //Kalder metoden i DBFacade
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1) //Pagination
    {
        return _dbFacade.GetCheepsFromAuthor(author, page); //Kalder metoden i DBFacade
    }
}
