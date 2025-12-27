using Chirp.Application.DTO;
using System.Security.Claims;

namespace Chirp.Application.Services.Interface
{
    public interface IIdentityUserService
    {
        bool IsSignedIn(ClaimsPrincipal User);

        Task<AuthorDTO?> GetCurrentIdentityAuthor(ClaimsPrincipal User);

        Task SignOutAsync();

    }
}
