using Chirp.Application.DTO;

namespace Chirp.Application.Services.Interface
{
    public interface IAuthorService
    {
        AuthorDTO? FindAuthorById(string name);
        IEnumerable<AuthorDTO?> GetFollowers(string authorId);
        IEnumerable<AuthorDTO?> GetFollowing(string authorId);

        bool IsFollowing(string authorId, string followeeId);

        void FollowAuthor(string authorId, string followeeId);

        void UnfollowAuthor(string authorId, string followeeId);

        int GetKarmaScore(string authorId);

        void ChangeKarma(int karma, string authorId);

    }
}
