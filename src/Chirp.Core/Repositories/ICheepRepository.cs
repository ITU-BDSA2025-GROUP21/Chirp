using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface ICheepRepository
    {
        /// <summary>
        /// Returns a paginated collection of all cheeps.
        /// </summary>
        /// <param name="page">The page number of results to retrieve. Must be greater than or equal to 1. Defaults to 1.</param>        
        /// <param name="pageSize">The pagination int i.e 32 as default</param>
        /// <returns>Returns an Enumerable collection of <see cref="Cheep"/> objects representing cheeps from all authors for the requested page.
        /// If no cheeps are found. the collection will be empty. </returns>
        IEnumerable<Cheep> GetAll(int page = 1, int pageSize = 32);

        /// <summary>
        /// Returns a paginated collection of cheeps specifically identified to be made by the same Author.
        /// </summary>
        /// <param name="authorId">The unique identification of the Author</param>
        /// <param name="page">The page number of results to retrieve. Must be grater than or equal to 1. Defaults to 1.</param>
        /// <param name="pageSize">The pagination int i.e 32 as default</param>
        /// <returns>Returns an Enumarable collection of <see cref="Cheep"/> objects representing cheeps from the specified <see cref="Author"/>,
        /// for the specified requested page. 
        /// If no cheeps are found. The collection will be empty.</returns>
        IEnumerable<Cheep> GetByAuthorId(string authorId, int page = 1, int pageSize = 32);

        /// <summary>
        /// Creates a new Cheep and saves it to the Database.
        /// </summary>
        /// <param name="text">The text you would like to save as the cheep-text</param>
        /// <param name="authorId">The unique identification of the Author</param>
        public void AddCheep(string text, string authorId);

        /// <summary>
        /// Returns a paginated collection of cheeps specifically identified to be made by multiple Authors.
        /// </summary>
        /// <param name="authorIds">The unique identification of the Author</param>
        /// <param name="page">The page number of results to retrieve. Must be grater than or equal to 1. Defaults to 1.</param>
        /// <param name="pageSize">The pagination int i.e 32 as default</param>
        /// <returns>Returns an Enumarable collection of <see cref="Cheep"/> objects representing cheeps from the specified <see cref="Author"/> 's,
        /// for the specified requested page. 
        /// If no cheeps are found. The collection will be empty.</returns>
        IEnumerable<Cheep> GetByMultipleAuthors(List<string> authorIds, int page = 1, int pageSize = 32);

        /// <summary>
        /// Registers or removes a "like" for the specified cheep by the given author.
        /// </summary>
        /// <remarks>This method updates the like status for the specified cheep and author. If the cheep
        /// or author does not exist, the operation may have no effect.</remarks>
        /// <param name="cheepId">The unique identifier of the cheep to be liked or unliked.</param>
        /// <param name="authorId">The identifier of the user performing the like or unlike action. Cannot be null or empty.</param>
        /// <param name="like">True to add a like to the cheep; False to remove an existing like.</param>
        public void Like(int cheepId, string authorID, bool like);

        /// <summary>
        /// Returns a specified Cheep from the CheepID
        /// </summary>
        /// <param name="id">The unique identifiaction of the Cheep.</param>
        /// <returns>Returns the requested <see cref="Cheep"/> from the CheepID</returns>
        public Cheep GetById(int id);

        /// <summary>
        /// Asynchronously retrieves a <see cref="Like"/> for the specified cheep and author, filtered by like status.
        /// </summary>
        /// <param name="cheepId">The unique identifier of the cheep to search for.</param>
        /// <param name="authorId">The unique identifier of the author whose like is being queried. Cannot be null.</param>
        /// <param name="like">True to retrieve a like; false to retrieve a dislike or unlike,
        /// depending on the application's model. </param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Like"/> if found;
        /// otherwise, null. </returns>
        Task<Like> GetLikeAsync(int cheepId, string authorId, bool like);
    }
}
