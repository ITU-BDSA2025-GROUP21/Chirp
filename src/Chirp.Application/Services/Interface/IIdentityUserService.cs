using Chirp.Application.DTO;
using System.Security.Claims;

namespace Chirp.Application.Services.Interface
{
    /// <summary>
    /// Defines a service contract for interacting with the authenticated
    /// identity user within the application.
    /// 
    /// This service abstracts access to authentication state and identity-related
    /// author information, providing a consistent way to determine whether a user
    /// is signed in, retrieve the currently authenticated author as a DTO, and
    /// perform sign-out operations.
    /// </summary>
    public interface IIdentityUserService
    {
        bool IsSignedIn(ClaimsPrincipal User);

        Task<AuthorDTO?> GetCurrentIdentityAuthor(ClaimsPrincipal User);

        Task SignOutAsync();

    }
}
