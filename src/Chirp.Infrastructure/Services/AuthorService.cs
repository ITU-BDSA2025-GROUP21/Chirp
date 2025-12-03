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

        public async Task RemoveAllFollowers(ClaimsPrincipal user)
        {

            var userAuthor = await _userManager.GetUserAsync(user);

            if(userAuthor == null)
            {
                return;
            }

            IEnumerable<Author> following = _repo.GetFollowing(userAuthor);

            foreach (var author in following)
            {
                _repo.UnfollowAuthor(userAuthor, author);

            }
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

        public async Task<IEnumerable<AuthorDTO>> GetFollowers(ClaimsPrincipal user)
        {
            if(!_signInManager.IsSignedIn(user))
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            Author author = await _userManager.GetUserAsync(user);

            return _repo.GetFollowers(author).Select(CreateAuthorDTO).ToList();
        }

        public async Task<IEnumerable<AuthorDTO>> GetFollowing(ClaimsPrincipal user)
        {
            if (!_signInManager.IsSignedIn(user))
            {
                return Enumerable.Empty<AuthorDTO>();
            }

            Author author = await _userManager.GetUserAsync(user);

            return _repo.GetFollowing(author).Select(CreateAuthorDTO).ToList();
        } 

        public async Task<bool> IsFollowing(ClaimsPrincipal followerUser, string followeeId)
        {
            if(!_signInManager.IsSignedIn(followerUser))
            {
                return false;
            }

            var follower = await _userManager.GetUserAsync(followerUser);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return false;
            }

            return _repo.DoesAuthorFollow(follower, followee);
        }

        public async Task FollowAuthor(ClaimsPrincipal followerUser, string followeeId)
        {
            if (!_signInManager.IsSignedIn(followerUser))
            {
                return;
            }

            var follower = await _userManager.GetUserAsync(followerUser);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.FollowAuthor(follower, followee);
        }

        public async Task UnfollowAuthor(ClaimsPrincipal followerUser, string followeeId)
        {
            if (!_signInManager.IsSignedIn(followerUser))
            {
                return;
            }

            var follower = await _userManager.GetUserAsync(followerUser);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.UnfollowAuthor(follower, followee);
        }
    }
}

