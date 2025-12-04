using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace nPlaywrightTests
{
    public class TestChirpWebFactory : IAsyncDisposable
    {
        private WebApplication? _app;
        private string? _tempDbPath;

        public string BaseAddress { get; private set; } = string.Empty;

        public async Task StartAsync()
        {
            if (_app != null)
            {
                return;
            }

            _tempDbPath = Path.Combine(Path.GetTempPath(), $"chirp-playwright-{Guid.NewGuid():N}.db");
            var connectionString = $"Data Source={_tempDbPath}";
            var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "Chirp.Web"));

            _app = global::Program.BuildWebApplication(
                new[] { "--urls", "http://127.0.0.1:0" },
                connectionString,
                disableHttpsRedirection: false,
                environmentName: Environments.Development,
                disableExternalAuth: true,
                contentRoot: contentRoot);
            await _app.StartAsync();

            var endpointDataSource = _app.Services.GetRequiredService<EndpointDataSource>();
            Console.WriteLine($"Endpoint count: {endpointDataSource.Endpoints.Count}");

            foreach (var endpoint in endpointDataSource.Endpoints.OfType<RouteEndpoint>())
            {
                Console.WriteLine($"Route pattern: {endpoint.RoutePattern.RawText}");
            }
            var address = _app.Urls.First();
            BaseAddress = address.EndsWith("/", StringComparison.Ordinal) ? address : address + "/";
        }

        public async ValueTask DisposeAsync()
        {
            if (_app == null)
            {
                return;
            }

            await _app.StopAsync();
            await _app.DisposeAsync();

            if (!string.IsNullOrWhiteSpace(_tempDbPath) && File.Exists(_tempDbPath))
            {
                try
                {
                    File.Delete(_tempDbPath);
                }
                catch (IOException)
                {
                    // If SQLite still has a handle, surface a clean test tear down.
                }
            }
        }
    }
}
