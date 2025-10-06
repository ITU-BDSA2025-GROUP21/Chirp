public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page = 1); //Pagination
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1); //Pagination
}

public class CheepService : ICheepService
{
    private readonly Chirp.Razor.DBFacade.DBFacade _dbFacade; //Database-Dependency

    public CheepService(Chirp.Razor.DBFacade.DBFacade dbFacade) //Constructor til Database-Dependency
    {
        _dbFacade = dbFacade;
    }

    public List<CheepViewModel> GetCheeps(int page = 1) //Pagination 
    {
        return _dbFacade.GetCheeps(page); //Kalder metoden i DBFacade
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page = 1) //Pagination
    {
        return _dbFacade.GetCheepsFromAuthor(author, page); //Kalder metoden i DBFacade
    }

}
