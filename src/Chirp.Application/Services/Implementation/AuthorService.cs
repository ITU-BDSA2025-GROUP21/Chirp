using Chirp.Application.DTO;
using Chirp.Application.Services.Interface;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Linq;

namespace Chirp.Application.Services.Implementation
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;

        public AuthorService(IAuthorRepository repo)
        {
            _repo = repo;
        }

        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if (author == null)
                return null;

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                karma = author.karma,
                Email = author.Email == null ? "noEmaiFound@nomail.dk" : author.Email,
                CreationDate = author.CreationDate.ToString("dd/MM/yyyy HH:mm"),
                ProfilePicPath = author.ProfilePicPath
            };
        }


        /// <summary>
        /// Retrieves the author with the specified authorID, if one exists.
        /// </summary>
        /// <param name="authorId">The unique authorID of the author to retrieve. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>An <see cref="AuthorDTO"/> representing the author with the specified authorID, or <see langword="null"/>
        /// if no such author exists.</returns>
        public AuthorDTO? FindAuthorById(string authorId)
        {
            var author = _repo.FindAuthorById(authorId);

            if (author == null)
            {
                return null;
            }

            return CreateAuthorDTO(author);
        }

        /// <summary>
        /// Retrieves a list of followers for the specified author.
        /// </summary>
        /// <param name ="authorId"> The unique authorID of the author to retrieve the list from. Cannot be <see langword="null"/> or empty.</param>
        /// <returns> Returns a list representing the followers of specified <see cref="Author"/>, or <see langword="null"/> </returns>
        public IEnumerable<AuthorDTO?> GetFollowers(string authorId)
        {
            Author? author = _repo.FindAuthorById(authorId);

            if (author == null) return Enumerable.Empty<AuthorDTO>();

            return _repo.GetFollowers(author).Select(CreateAuthorDTO).ToList();
        }

        /// <summary>
        /// Retrieves a list of following for the specified author.
        /// </summary>
        /// <param name ="authorId"> The unique authorID of the author to retrieve the list from. Cannot be <see langword="null"/> or empty.</param>
        /// <returns> Returns a list representing the following of specified <see cref="Author"/>, or <see langword="null"/> </returns>
        public IEnumerable<AuthorDTO?> GetFollowing(string authorId)
        {
            Author? author = _repo.FindAuthorById(authorId);

            if (author == null) return Enumerable.Empty<AuthorDTO>();

            return _repo.GetFollowing(author).Select(CreateAuthorDTO).ToList();
        }

        /// <summary>
        /// Is author with authorId following author with followeeId-boolean.
        /// </summary>
        /// <param name="authorId"></param> The unique authorID of the author who might be following.
        /// <param name="followeeId"></param> The unique authorID of the author who might be followed.
        /// <returns> Returns a boolean, true if AuthorID is following followeeID, false if not.</returns>
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

        public int GetKarmaScore(string authorId)
        {
            return _repo.GetKarmaScore(authorId);
        }

        public void ChangeKarma(int karma, string authorId)
        {
            _repo.ChangeKarma(karma, authorId);
        }
    }
}
