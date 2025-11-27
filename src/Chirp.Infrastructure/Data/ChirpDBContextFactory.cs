using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Chirp.Core.Data
{
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