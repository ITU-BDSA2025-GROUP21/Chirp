using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Chirp.Core.Data
{
    /// <summary>
    /// Provides a design-time factory for creating instances of
    /// <see cref="ChirpDBContext"/>.
    /// 
    /// This factory is used exclusively by Entity Framework Core tooling
    /// (such as migrations and database updates) at design time, when the
    /// application's normal dependency injection setup is not available (Because we are using Onion-Structure).
    /// 
    /// It manually configures the DbContext by loading configuration from
    /// the Chirp.Web project's appsettings.json file and supplying the
    /// required database provider and connection string.
    /// 
    /// This class is not used at runtime
    /// </summary>
    public class ChirpDBContextFactory : IDesignTimeDbContextFactory<ChirpDBContext>
    {
        public ChirpDBContext CreateDbContext(string[] args)
        {
            // Find configurationen
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Chirp.Web"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("ChirpDBConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ChirpDBContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new ChirpDBContext(optionsBuilder.Options);
        }
    }
}