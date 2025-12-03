using Chirp.Core.DTO;

namespace Chirp.Core.Services
{
    public interface IAuthorService
    {
        AuthorDTO? FindAuthorByEmail(string email);
        AuthorDTO? FindAuthorByName(string name);

        IEnumerable<AuthorDTO> GetFollowers(string name);
        IEnumerable<AuthorDTO> GetFollowing(string name);

        bool IsFollowing(string followerName, string followeeName);
        void FollowAuthor(string followerName, string followeeName);
        void UnfollowAuthor(string followerName, string followeeName);
    }
}
