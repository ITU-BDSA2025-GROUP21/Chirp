using Chirp.Core.DTO;
using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface IAuthorRepository
    {
        public Author? FindAuthorById(string id);
        Task DeleteAuthorByIdAsync(string authorId);
        IEnumerable<Author> GetFollowers(Author author);  
        IEnumerable<Author> GetFollowing(Author author);
        bool DoesAuthorFollow(Author Follower, Author Followee);
        void FollowAuthor(Author Follower, Author Followee);
        void UnfollowAuthor(Author Follower, Author Followee);
        int getKarmaScore(string authorId);
        void changeKarma(int karma, string authorId);
    }
}