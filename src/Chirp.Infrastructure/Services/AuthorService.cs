using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using System.Linq;

namespace Chirp.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;

        public AuthorService(IAuthorRepository repo) {
            _repo = repo;
        }

        public AuthorDTO? FindAuthorByName(string name)
        {
            var author = _repo.FindAuthorByName(name);

            if (author == null)
            {
                return null;
            }

            return createAuthorDTO(author);
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            var author = _repo.FindAuthorByEmail(email);

            if (author == null)
            {
                return null;
            }

            return createAuthorDTO(author);
        }

        public IEnumerable<AuthorDTO> GetFollowers(string name)
        {
            var author = _repo.FindAuthorByName(name);

            if (author == null)
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            return _repo.GetFollowers(author).Select(createAuthorDTO).ToList();
        }

        public IEnumerable<AuthorDTO> GetFollowing(string name)
        {
            var author = _repo.FindAuthorByName(name);

            if (author == null)
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            return _repo.GetFollowing(author).Select(createAuthorDTO).ToList();
        } 

        public bool IsFollowing(string followerName, string followeeName)
        {
            var follower = _repo.FindAuthorByName(followerName);
            var followee = _repo.FindAuthorByName(followeeName);

            if (follower == null || followee == null)
            {
                return false;
            }

            return _repo.DoesAuthorFollow(follower, followee);
        }

        public void FollowAuthor(string followerName, string followeeName)
        {
            var follower = _repo.FindAuthorByName(followerName);
            var followee = _repo.FindAuthorByName(followeeName);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.FollowAuthor(follower, followee);
        }

        public void UnfollowAuthor(string followerName, string followeeName) 
        {
            var follower = _repo.FindAuthorByName(followerName);
            var followee = _repo.FindAuthorByName(followeeName);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.UnfollowAuthor(follower, followee);
        }

        private readonly Func<Author, AuthorDTO> createAuthorDTO =
        author => new AuthorDTO
        {
            Name = author.Name,
            Email = author.Email
        };

        private readonly Func<UserFollow, UserFollowDTO> createUserFollowDTO =
        userFollow => new UserFollowDTO
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId,
            TimeStamp = userFollow.TimeStamp.ToString("dd/MM/yyyy HH:mm")
        };
    }
}

