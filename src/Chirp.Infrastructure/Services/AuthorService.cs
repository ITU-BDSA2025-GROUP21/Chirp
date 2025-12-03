using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Linq;

namespace Chirp.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;
        private readonly SignInManager<Author> _signInManager;
        private readonly UserManager<Author> _userManager;
        public AuthorService(IAuthorRepository repo, SignInManager<Author> signInManager, UserManager<Author> userManager) {
            _repo = repo;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public AuthorDTO? FindAuthorByName(string name)
        {
            var author = _repo.FindAuthorByName(name);
            return CreateAuthorDTO(author);
        }

        public AuthorDTO? GetCurrentIdentityAuthor(ClaimsPrincipal user)
        {
            return CreateAuthorDTO(_userManager.GetUserAsync(user).Result);


        }

        public bool SignIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        public Task DeleteAuthorByIdAsync(string authorId)
            => _repo.DeleteAuthorByIdAsync(authorId);

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if (author == null)
                return null;

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                CreationDate = author.CreationDate.ToString("dd/MM/yyyy HH:mm")
            };
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            var author = _repo.FindAuthorByEmail(email);

            if (author == null)
            {
                return null;
            }

            return CreateAuthorDTO(author);
        }

        public IEnumerable<AuthorDTO> GetFollowers(string name)
        {
            var author = _repo.FindAuthorByName(name);

            if (author == null)
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            return _repo.GetFollowers(author).Select(CreateAuthorDTO).ToList();
        }

        public IEnumerable<AuthorDTO> GetFollowing(string name)
        {
            var author = _repo.FindAuthorByName(name);

            if (author == null)
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            return _repo.GetFollowing(author).Select(CreateAuthorDTO).ToList();
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

        private readonly Func<UserFollow, UserFollowDTO> createUserFollowDTO =
        userFollow => new UserFollowDTO
        {
            FollowerId = userFollow.FollowerId,
            FolloweeId = userFollow.FolloweeId,
            TimeStamp = userFollow.TimeStamp.ToString("dd/MM/yyyy HH:mm")
        };
    }
}

