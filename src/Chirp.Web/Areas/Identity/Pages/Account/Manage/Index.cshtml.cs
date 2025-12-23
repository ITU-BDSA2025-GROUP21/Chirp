// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Linq;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manager
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Chirp.Core.Models.Author> _userManager;
        private readonly SignInManager<Chirp.Core.Models.Author> _signInManager;
        private readonly IWebHostEnvironment _env;
        private const string DefaultProfilePic = "/images/default_profile_pic.png";

        public IndexModel(
            UserManager<Chirp.Core.Models.Author> userManager,
            SignInManager<Chirp.Core.Models.Author> signInManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }
        public string CurrentProfilePic { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Upload new profile picture")]
            public IFormFile ProfilePic { get; set; }

            [Display(Name = "Remove profile picture (revert to default)")]
            public bool RemoveProfilePic { get; set; }
        }

        private async Task LoadAsync(Chirp.Core.Models.Author user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            CurrentProfilePic = string.IsNullOrWhiteSpace(user.ProfilePicPath) ? DefaultProfilePic : user.ProfilePicPath;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Handle avatar removal
            if (Input.RemoveProfilePic)
            {
                user.ProfilePicPath = DefaultProfilePic;
            }

            // Handle avatar upload
            if (Input.ProfilePic != null && Input.ProfilePic.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "profilePics");
                Directory.CreateDirectory(uploads);

                var ext = Path.GetExtension(Input.ProfilePic.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowed.Contains(ext) || Input.ProfilePic.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Input.Avatar", "Only jpg, jpeg, png, webp up to 2 MB are allowed.");
                    await LoadAsync(user);
                    return Page();
                }

                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await Input.ProfilePic.CopyToAsync(stream);
                }

                user.ProfilePicPath = $"/profilePics/{fileName}";
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
