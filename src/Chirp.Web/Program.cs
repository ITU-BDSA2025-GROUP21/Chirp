using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Services;
using Chirp.Razor.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();


builder.Services.AddDbContext<ChirpDBContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ChirpDBConnection")));

builder.Services.AddDefaultIdentity<Author>(
    options => { 
        options.SignIn.RequireConfirmedAccount = false; 
        options.User.RequireUniqueEmail = true; 
    }).AddEntityFrameworkStores<ChirpDBContext>();

builder.Services.AddAuthentication().AddGitHub(options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
    options.Scope.Add("user:email");

    options.ClaimActions.MapJsonKey("urn:github:name", "name");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();

    ctx.Database.Migrate();
    DbInitializer.SeedDatabase(ctx);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
