using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;

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
            return CreateAuthorDTO(_repo.FindAuthorByName(name));
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            return CreateAuthorDTO(_repo.FindAuthorByEmail(email));
        }


        public IEnumerable<AuthorDTO> GetFollowers(string name)
        {
            return null;
        }

        public IEnumerable<AuthorDTO> GetFollowing(string name)
        {
            return null;
        } 

        public bool IsFollowing(string followerName, string followeeName)
        {
            return false;
        }

        public bool FollowAuthor(string followerName, string followeeName)
        {
            return false;
        }

        public bool UnfollowAuthor(string followerName, string followeeName) 
        {
            return false;
        }

        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if(author == null)
            {
                return null;
            }

            return new AuthorDTO
            {
                Name = author.Name,
                Email = author.Email
            };
        }

        private UserFollowDTO CreateUserFollowDTO(UserFollow userFollow)
        {
            if (userFollow == null)
            {
                return null;
            }

            return new UserFollowDTO
            {
                FollowerId = userFollow.FollowerId,
                FolloweeId = userFollow.FolloweeId,
                TimeStamp = userFollow.TimeStamp.ToString("dd/MM/yyyy HH:mm")
            };
        }
    }
}

