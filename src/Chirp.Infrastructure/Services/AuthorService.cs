using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace Chirp.Infrastructure.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _repo;
        private readonly SignInManager<Author> _signInManager;
        private readonly UserManager<Author> _userManager;
        public AuthorService(IAuthorRepository repo, SignInManager<Author> signInManager, UserManager<Author> userManager) {
            _repo = repo;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public AuthorDTO? FindAuthorByName(string name)
        {
            return CreateAuthorDTO(_repo.FindAuthorByName(name));
        }

        public AuthorDTO? FindAuthorByEmail(string email)
        {
            return CreateAuthorDTO(_repo.FindAuthorByEmail(email));
        }

        public AuthorDTO? GetCurrentIdentityAuthor(ClaimsPrincipal user)
        {
            return CreateAuthorDTO(_userManager.GetUserAsync(user).Result);


        }

        public bool SignIn(ClaimsPrincipal user)
        {
            return _signInManager.IsSignedIn(user);
        }

        public Task DeleteAuthorByIdAsync(string authorId)
            => _repo.DeleteAuthorByIdAsync(authorId);

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }


        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if(author == null)
            {
                return null;
            }

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Email = author.Email,
                CreationDate = author.CreationDate.ToString("dd/MM/yyyy HH:mm")
            };
        }
    }

}

