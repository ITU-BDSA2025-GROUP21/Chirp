using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Razor.Repositories;
using Chirp.Core.Services.Interfaces;
using Chirp.Infrastructure.Data;
using Chirp.Core.Repositories;


public class TestServices : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly SqliteConnection _conn;

    public ICheepService CheepService { get; }

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
            DbInitializer.SeedDatabase(ctx);
        }

        CheepService = _provider.GetRequiredService<ICheepService>();
    }

    public void Dispose()
    {
        _provider.Dispose();
        _conn.Close();
        _conn.Dispose();
    }
}
