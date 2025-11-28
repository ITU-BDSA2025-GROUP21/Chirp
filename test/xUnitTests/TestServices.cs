using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Razor.Repositories;
using Chirp.Core.Services;
using Chirp.Core.Data;
using Chirp.Core.Repositories;
using Chirp.Core.Models;
using Chirp.Infrastructure.Services;
using System.Security.Policy;

public class TestServices : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly SqliteConnection _conn;
    private readonly IServiceScope _scope;

    public ChirpDBContext ctx { get; }
    public ICheepRepository _cheepRepository { get; }
    public ICheepService _cheepService { get; }
    public IAuthorRepository _authorRepository { get; }
    public IAuthorService _authorService { get; }

    public TestServices()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var services = new ServiceCollection();

        services.AddDbContext<ChirpDBContext>(o => o.UseSqlite(_conn));

        services.AddScoped<ICheepRepository, CheepRepository>();
        services.AddScoped<ICheepService, CheepService>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IAuthorService, AuthorService>();

        _provider = services.BuildServiceProvider();

        _scope = _provider.CreateScope();

        ctx = _scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
        ctx.Database.EnsureCreated();
        DbInitializer.SeedDatabase(ctx);
        
        _cheepService = _scope.ServiceProvider.GetRequiredService<ICheepService>();
        _cheepRepository = _scope.ServiceProvider.GetRequiredService<ICheepRepository>();
        _authorService = _scope.ServiceProvider.GetRequiredService<AuthorService>();
        _authorRepository = _scope.ServiceProvider.GetRequiredService<IAuthorRepository>();

    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        _conn.Close();
        _conn.Dispose();
    }
}