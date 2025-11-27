using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Razor.Repositories;
using Chirp.Core.Services;
using Chirp.Infrastructure.Data;
using Chirp.Core.Repositories;
using Chirp.Infrastructure.Models;

public class TestServices : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly SqliteConnection _conn;
    private readonly DbContext _dbContext;

    public ICheepService CheepService { get; }
    public ICheepRepository CheepRepository { get; }

    public TestServices()
    {
        _conn = new SqliteConnection("Data Source=:memory:");
        _conn.Open();

        var services = new ServiceCollection();

        services.AddDbContext<ChirpDBContext>(o => o.UseSqlite(_conn));

        services.AddScoped<ICheepRepository, CheepRepository>();

        services.AddScoped<ICheepService, CheepService>();

        _provider = services.BuildServiceProvider();

        using (var scope = _provider.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<ChirpDBContext>();
            ctx.Database.EnsureCreated();
            _dbContext = ctx;
            DbInitializer.SeedDatabase(ctx);
        }

        CheepService = _provider.GetRequiredService<ICheepService>();
        CheepRepository = _provider.GetRequiredService<ICheepRepository>();
    }

    public DbContext GetDbContext()
    {
        return _dbContext;
    }

    public void Dispose()
    {
        _provider.Dispose();
        _conn.Close();
        _conn.Dispose();
    }
}