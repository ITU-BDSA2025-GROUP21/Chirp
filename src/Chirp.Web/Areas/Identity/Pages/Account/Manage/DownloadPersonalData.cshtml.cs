// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Chirp.Core.Data;
using Chirp.Core.Models;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manager
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<Chirp.Core.Models.Author> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;
        private readonly ChirpDBContext _dbContext;

        public DownloadPersonalDataModel(
            UserManager<Chirp.Core.Models.Author> userManager,
            ILogger<DownloadPersonalDataModel> logger,
            ChirpDBContext dbContext)
        {
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult OnGet()
        {
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _dbContext.Entry(user).Collection(u => u.Cheeps).LoadAsync();

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            var personalData = new Dictionary<string, object>();
            var personalDataProps = typeof(Author).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData[p.Name] = p.GetValue(user) ?? "null";
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            if (user.Cheeps != null)
            {
                personalData["Cheeps"] = user.Cheeps.Select(c => new
                {
                    Message = c.Text,
                    Timestamp = c.TimeStamp.ToString("dd/MM/yyyy HH:mm")
                });
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(
                JsonSerializer.SerializeToUtf8Bytes(personalData, new JsonSerializerOptions { WriteIndented = true }),
                "application/json");
        }
    }
}

