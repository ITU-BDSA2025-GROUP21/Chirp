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
        public AuthorService(IAuthorRepository repo) {
            _repo = repo;
        }

        public void RemoveAllFollowers(string authorId)
        {

            var userAuthor = _repo.FindAuthorById(authorId);

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

        public async Task DeleteAuthorByIdAsync(string authorId)
            => await _repo.DeleteAuthorByIdAsync(authorId);

       
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

        public AuthorDTO? FindAuthorById(string authorId)
        {
            var author = _repo.FindAuthorById(authorId);

            if (author == null)
            {
                return null;
            }

            return CreateAuthorDTO(author);
        }

        public IEnumerable<AuthorDTO> GetFollowers(string authorId)
        {
            Author author = _repo.FindAuthorById(authorId);

            if (author == null) return Enumerable.Empty<AuthorDTO>();

            return _repo.GetFollowers(author).Select(CreateAuthorDTO).ToList();
        }

        public IEnumerable<AuthorDTO> GetFollowing(string authorId)
        {
            Author author = _repo.FindAuthorById(authorId);

            if (author == null) return Enumerable.Empty<AuthorDTO>();

            return _repo.GetFollowing(author).Select(CreateAuthorDTO).ToList();
        } 

        public bool IsFollowing(string authorId, string followeeId)
        {
            var follower = _repo.FindAuthorById(authorId);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return false;
            }

            return _repo.DoesAuthorFollow(follower, followee);
        }

        public void FollowAuthor(string authorId, string followeeId)
        {
            var follower = _repo.FindAuthorById(authorId);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.FollowAuthor(follower, followee);
        }

        public void UnfollowAuthor(string authorId, string followeeId)
        {
            var follower = _repo.FindAuthorById(authorId);
            var followee = _repo.FindAuthorById(followeeId);

            if (follower == null || followee == null)
            {
                return;
            }

            _repo.UnfollowAuthor(follower, followee);
        }
    }
}

