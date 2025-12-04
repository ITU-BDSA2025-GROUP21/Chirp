using Chirp.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Core.Services
{
    public interface IIdentityUserService
    {


        bool IsSignedIn(ClaimsPrincipal User);

        Task<AuthorDTO?> GetCurrentIdentityAuthor(ClaimsPrincipal User);

        Task SignOutAsync();

    }
}
