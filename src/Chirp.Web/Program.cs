using System;
using System.IO;
using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Razor.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Chirp.Application.Services.Implementation;
using Chirp.Application.Services.Interface;

var app = Program.BuildWebApplication(args);
app.Run();

public partial class Program
{
    public static WebApplication BuildWebApplication(
        string[]? args = null,
        string? connectionStringOverride = null,
        bool disableHttpsRedirection = false,
        string? environmentName = null,
        bool disableExternalAuth = false,
        string? contentRoot = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args ?? Array.Empty<string>(),
            ContentRootPath = contentRoot ?? Directory.GetCurrentDirectory(),
            ApplicationName = typeof(Program).Assembly.GetName().Name
        });

        if (!string.IsNullOrWhiteSpace(contentRoot))
        {
            builder.Environment.ContentRootPath = contentRoot;
            builder.Configuration.SetBasePath(contentRoot);
        }

        if (!string.IsNullOrWhiteSpace(connectionStringOverride))
        {
            builder.Configuration["ConnectionStrings:ChirpDBConnection"] = connectionStringOverride;
        }

        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            builder.Environment.EnvironmentName = environmentName;
        }

        // Add services to the container.
        builder.Services.AddRazorPages(options =>
        {
            // Root route is handled by MapFallbackToPage("/PublicView"); avoid duplicate endpoint.
        });
        builder.Services.AddControllersWithViews();
        builder.Services.AddScoped<ICheepService, CheepService>();
        builder.Services.AddScoped<IIdentityUserService, IdentityUserService>();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorService, AuthorService>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();


builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None);

builder.Services.AddDbContext<ChirpDBContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("ChirpDBConnection"))
           .EnableSensitiveDataLogging(false);
});

        builder.Services.AddDefaultIdentity<Author>(
            options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ChirpDBContext>();

        if (!disableExternalAuth)
        {
            builder.Services.AddAuthentication().AddGitHub(options =>
            {
                options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
                options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
                options.Scope.Add("user:email");

                options.ClaimActions.MapJsonKey("urn:github:name", "name");
            });
        }
        else
        {
            builder.Services.AddAuthentication();
        }

        var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

    ctx.Database.Migrate();
}

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        if (!disableHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.MapFallbackToPage("/PublicView");

        return app;
    }
}
