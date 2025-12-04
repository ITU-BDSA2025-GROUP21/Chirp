using Chirp.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Services
{
    public interface IAuthorService
    {
        AuthorDTO? FindAuthorById(string name);

        Task DeleteAuthorByIdAsync(string authorId);
        IEnumerable<AuthorDTO> GetFollowers(string authorId);

        IEnumerable<AuthorDTO> GetFollowing(string authorId);
        void RemoveAllFollowers(string authorId);


        bool IsFollowing(string authorId, string followeeId);

        void FollowAuthor(string authorId, string followeeId);

        void UnfollowAuthor(string authorId, string followeeId);

    }
}
