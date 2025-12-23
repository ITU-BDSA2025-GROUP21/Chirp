using Chirp.Core.Data;
using Chirp.Core.Models;
using Chirp.Core.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Services;
using Chirp.Razor.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using xUnitTests.Seed;

public class TestServices : IDisposable
{
    private readonly SqliteConnection _conn;
    private readonly IServiceScope _scope;
    private readonly ServiceProvider _provider;

    public ChirpDBContext ctx { get; }
    public ICheepRepository _cheepRepository { get; }
    public ICheepService _cheepService { get; }
    public IAuthorRepository _authorRepository { get; }
    public IAuthorService _authorService { get; }
    public UserManager<Author> _userManager { get; }

    public TestServices()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var services = new ServiceCollection();

        services.AddDbContext<ChirpDBContext>(o => o.UseSqlite(_conn));

        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddScoped<ICheepService, CheepService>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();

        services.AddScoped<IUserStore<Author>, UserStore<Author, IdentityRole, ChirpDBContext>>();

        //setup identity services
        services.AddIdentityCore<Author>(options => { });
        services.AddScoped<UserManager<Author>>();
        services.AddScoped<SignInManager<Author>>();

        services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserClaimsPrincipalFactory<Author>, UserClaimsPrincipalFactory<Author>>();
        services.AddScoped<IOptions<IdentityOptions>, OptionsManager<IdentityOptions>>();
        services.AddScoped<ILogger<SignInManager<Author>>, Logger<SignInManager<Author>>>();
        services.AddScoped<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
        services.AddScoped<IUserConfirmation<Author>, DefaultUserConfirmation<Author>>();
        services.AddScoped<SignInManager<Author>>();
        services.AddScoped<IAuthorService, AuthorService>();

        _provider = services.BuildServiceProvider();

        _scope = _provider.CreateScope();

        ctx = _scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
        ctx.Database.EnsureCreated();
        DbInitializer.SeedDatabase(ctx);

        _cheepService = _scope.ServiceProvider.GetRequiredService<ICheepService>();
        _cheepRepository = _scope.ServiceProvider.GetRequiredService<ICheepRepository>();
        _authorService = _scope.ServiceProvider.GetRequiredService<IAuthorService>();
        _authorRepository = _scope.ServiceProvider.GetRequiredService<IAuthorRepository>();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<Author>>();
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        _conn.Close();
        _conn.Dispose();
    }
}