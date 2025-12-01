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

        AuthorDTO? FindAuthorByEmail(string email);
        AuthorDTO? FindAuthorByName(string name);
        AuthorDTO? GetCurrentIdentityAuthor(ClaimsPrincipal user);

        bool SignIn(ClaimsPrincipal user);

        Task DeleteAuthorByIdAsync(string authorId);
        Task SignOutAsync();

    }
}
