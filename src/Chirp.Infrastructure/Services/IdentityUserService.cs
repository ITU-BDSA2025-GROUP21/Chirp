using Chirp.Core.DTO;
using Chirp.Core.Models;
using Chirp.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chirp.Infrastructure.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<Author> _userManager;
        private readonly SignInManager<Author> _signInManager;


        public IdentityUserService(SignInManager<Author> signInManager, UserManager<Author> userManager) {
            _userManager = userManager;
            _signInManager = signInManager;

        }



        public bool IsSignedIn(ClaimsPrincipal User)
        {
            return _signInManager.IsSignedIn(User);
        }

        public async Task<AuthorDTO?> GetCurrentIdentityAuthor(ClaimsPrincipal User)
        {
            if (!IsSignedIn(User)) return null;

            Author? authorModel = await _userManager.GetUserAsync(User);

            if (authorModel == null)
                return null;

            return CreateAuthorDTO(authorModel);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();   
        }

        private AuthorDTO? CreateAuthorDTO(Author author)
        {
            if (author == null)
                return null;

            return new AuthorDTO
            {
                Id = author.Id,
                Name = author.Name,
                Email = (author.Email == null ? "noEmaiFound@nomail.dk" : author.Email  ),
                CreationDate = author.CreationDate.ToString("dd/MM/yyyy HH:mm")
            };
        }
    }
}
