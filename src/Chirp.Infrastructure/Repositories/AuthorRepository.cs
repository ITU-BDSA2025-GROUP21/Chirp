using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.DTO;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ChirpDBContext _context;

        public AuthorRepository(ChirpDBContext context)
        {
            _context = context;
        }

        public Author? FindAuthorByName(string name)
        {
            return _context.Authors
                .Where(a => a.Name.ToLower() == name.ToLower())
                .FirstOrDefault();
        }

        public Author? FindAuthorById(string id)
        {
            return _context.Authors
                .Where(a => a.Id == id)
                .FirstOrDefault();
        }

        public Author? FindAuthorByEmail(string email)
        {
            return _context.Authors
                .Where(a => a.Email.ToLower() == email.ToLower())
                .FirstOrDefault();
        }

        public async Task DeleteAuthorByIdAsync(string authorId)
        {
            await _context.Authors
                .Where(a => a.Id == authorId)
                .ExecuteDeleteAsync();
        }

        public IEnumerable<Author> GetFollowers(Author author) 
        {
            return _context.UserFollows
                .Where(x => x.FolloweeId == author.Id)
                .Include(x => x.Follower)
                .Select(x => x.Follower)
                .ToList();
        }

        public IEnumerable<Author> GetFollowing(Author author) 
        {
            return _context.UserFollows
                .Where(x => x.FollowerId == author.Id)
                .Include(x => x.Followee)
                .Select(x => x.Followee)
                .ToList();
        }

        public bool DoesAuthorFollow(Author Follower, Author Followee) 
        {
            return _context.UserFollows.Any(x =>
                x.FollowerId == Follower.Id &&
                x.FolloweeId == Followee.Id);
        }

        public void FollowAuthor(Author Follower, Author Followee) 
        {
            if (DoesAuthorFollow(Follower, Followee))
            {
                return;
            }

            _context.UserFollows.Add(new UserFollow
            {
                FollowerId = Follower.Id,
                FolloweeId = Followee.Id,
                TimeStamp = DateTime.UtcNow
            });

            _context.SaveChanges();
        }

        public void UnfollowAuthor(Author Follower, Author Followee) 
        {
            var follow = _context.UserFollows.FirstOrDefault(x =>
                x.FollowerId == Follower.Id &&
                x.FolloweeId == Followee.Id);

            if (follow == null)
            {
                return;
            }

            _context.UserFollows.Remove(follow);

            _context.SaveChanges();
        }
    }
}