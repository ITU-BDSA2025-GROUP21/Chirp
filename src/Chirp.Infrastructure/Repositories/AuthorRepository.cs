using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
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

        /// <summary>
        /// Retrieves the author with the specified identifier, if one exists.
        /// </summary>
        /// <param name="id">The unique identifier of the author to find. Cannot be null or empty.</param>
        /// <returns>— The <see cref="Author"/> with the specified identifier, or null if no matching author is
        /// found.</returns>
        public Author? FindAuthorById(string id)
        {
            return _context.Authors
                .Where(a => a.Id == id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the collection of authors who follow the specified author.
        /// </summary>
        /// <param name="author">The author whose followers are to be retrieved. Cannot be null.</param>
        /// <returns>An enumerable collection of <see cref="Author"/> objects representing the followers of the specified author.
        /// The collection is empty if the author has no followers.</returns>
        public IEnumerable<Author> GetFollowers(Author author) 
        {
            return _context.UserFollows
                .Where(x => x.FolloweeId == author.Id)
                .Include(x => x.Follower)
                .Select(x => x.Follower)
                .ToList();
        }

        /// <summary>
        /// Returns a collection of authors that the specified author is following.
        /// </summary>
        /// <param name="author">The author whose followed authors are to be retrieved. Cannot be null.</param>
        /// <returns>An enumerable collection of <see cref="Author"/> objects representing the authors followed by the specified
        /// author. The collection is empty if the author is not following anyone.</returns>
        public IEnumerable<Author> GetFollowing(Author author) 
        {
            return _context.UserFollows
                .Where(x => x.FollowerId == author.Id)
                .Include(x => x.Followee)
                .Select(x => x.Followee)
                .ToList();
        }

        /// <summary>
        /// Determines whether the specified author is following another author.
        /// </summary>
        /// <param name="Follower">The author whose following status is to be checked.</param>
        /// <param name="Followee">The author to check if they are being followed by <paramref name="Follower"/>.</param>
        /// <returns>true if <paramref name="Follower"/> is following <paramref name="Followee"/>; otherwise,
        /// false.</returns>
        public bool DoesAuthorFollow(Author Follower, Author Followee) 
        {
            return _context.UserFollows.Any(x =>
                x.FollowerId == Follower.Id &&
                x.FolloweeId == Followee.Id);
        }
        
        /// <summary>
        /// Adds a follow relationship between the specified follower and followee authors.
        /// </summary>
        /// <remarks>If the follower is already following the followee, this method performs no
        /// action.</remarks>
        /// <param name="Follower">The author who will follow the specified followee. Cannot be null.</param>
        /// <param name="Followee">The author to be followed. Cannot be null.</param>
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

        /// <summary>
        /// Removes the follow relationship between the specified follower and followee authors.
        /// </summary>
        /// <remarks>If the follower is not currently following the followee, this method performs no
        /// action.</remarks>
        /// <param name="Follower">The author who is unfollowing another author. Cannot be <c>null</c>.</param>
        /// <param name="Followee">The author to be unfollowed. Cannot be <c>null</c>.</param>
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

        public int GetKarmaScore(string authorId) 
        {
            var author = _context.Authors
                .Where(a => a.Id == authorId)
                .FirstOrDefault();
            if (author == null)
            {
                return 0;
            }
            return author.karma;
        }

        public void ChangeKarma(int karma, string authorId) 
        {
            var author = _context.Authors
                .Where(a => a.Id == authorId)
                .FirstOrDefault();
            if (author == null)
            {
                return;
            }
            author.karma += karma;
            _context.SaveChanges();
        }
    }
}
