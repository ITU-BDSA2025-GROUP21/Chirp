using Chirp.Core.Models;

namespace Chirp.Core.Repositories
{
    public interface IAuthorRepository
    {
        public Author? FindAuthorById(string id);
        IEnumerable<Author> GetFollowers(Author author);  
        IEnumerable<Author> GetFollowing(Author author);
        bool DoesAuthorFollow(Author Follower, Author Followee);
        void FollowAuthor(Author Follower, Author Followee);
        void UnfollowAuthor(Author Follower, Author Followee);
        int GetKarmaScore(string authorId);
        void ChangeKarma(int karma, string authorId);
    }
}
