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

        AuthorDTO? GetCurrentIdentityAuthor(ClaimsPrincipal user);

        bool SignIn(ClaimsPrincipal user);

        Task DeleteAuthorByIdAsync(string authorId);
        Task SignOutAsync();
        Task<IEnumerable<AuthorDTO>> GetFollowers(ClaimsPrincipal user);

        Task<IEnumerable<AuthorDTO>> GetFollowing(ClaimsPrincipal user);
        Task RemoveAllFollowers(ClaimsPrincipal user);


        Task<bool> IsFollowing(ClaimsPrincipal followerUser, string followeeId);

        Task FollowAuthor(ClaimsPrincipal followerUser, string followeeId);

        Task UnfollowAuthor(ClaimsPrincipal followerUser, string followeeId);


    }
}
