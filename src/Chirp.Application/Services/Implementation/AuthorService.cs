using Chirp.Application.DTO;
using Chirp.Application.Services.Interface;
using Chirp.Core.Models;
using Chirp.Core.Repositories;

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

        public IEnumerable<AuthorDTO?> GetFollowers(string authorId)
        {
            Author? author = _repo.FindAuthorById(authorId);

            if (author == null) return Enumerable.Empty<AuthorDTO>();

            return _repo.GetFollowers(author).Select(CreateAuthorDTO).ToList();
        }

        public IEnumerable<AuthorDTO?> GetFollowing(string authorId)
        {
            Author? author = _repo.FindAuthorById(authorId);

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
