using ChirpProject.MainApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MyApp.Tests
{
    public class AppTests
    {
        private readonly App _app;
        private readonly FakeServerHandler _handler;

        public AppTests()
        {
            // Create the fake server handler which keeps an in-memory list of cheeps.
            _handler = new FakeServerHandler();
            var httpClient = new HttpClient(_handler)
            {
                BaseAddress = new Uri("http://test/") // base address used by the handler for route parsing
            };

            // Inject the fake client into App
            _app = new App(httpClient, "http://test/");

            // Ensure starting each test with empty state
            _handler.Purge();
        }

        [Fact]
        public void SendCheep()
        {
            _app.SendCheep("HelloWorld");

            var cheeps = _app.GetCheepAsyncJson().GetAwaiter().GetResult();
            Assert.Contains(cheeps, c => c.Message == "HelloWorld");
        }

        [Fact]
        public void SendCheepWhitespace()
        {
            _handler.Purge();

            _app.SendCheep(" ");
            _app.SendCheep("");
            var cheeps = _app.GetCheepAsyncJson().GetAwaiter().GetResult();
            Assert.True(cheeps.Count == 0);
        }

        [Fact]
        public void PurgeCheeps()
        {
            _app.SendCheep("HelloWorld");
            _app.SendCheep("HelloWorld2");
            _handler.Purge();
            var cheeps = _app.GetCheepAsyncJson().GetAwaiter().GetResult();
            Assert.True(cheeps.Count == 0);
        }

        [Fact]
        public void ReadCheeps()
        {
            _app.SendCheep("HelloWorld");
            _app.SendCheep("HelloWorld2");
            var cheeps = _app.GetCheepAsyncJson().GetAwaiter().GetResult();
            Assert.True(cheeps.Count == 2);
        }
    }

    // Small in-test fake server implemented as an HttpMessageHandler.
    internal class FakeServerHandler : HttpMessageHandler
    {
        private readonly List<ChirpProject.MainApp.Cheep> _store = new();

        public void Purge()
        {
            _store.Clear();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = request.RequestUri.AbsolutePath.Trim('/');
            var response = new HttpResponseMessage();

            if (request.Method == HttpMethod.Post && path.Equals("cheep", StringComparison.OrdinalIgnoreCase))
            {
                // read cheep from body
                var cheep = request.Content.ReadFromJsonAsync<ChirpProject.MainApp.Cheep>(cancellationToken).GetAwaiter().GetResult();
                if (cheep != null)
                {
                    // store
                    _store.Add(cheep);
                    response.StatusCode = HttpStatusCode.Created;
                    response.ReasonPhrase = "Created";
                }
                else
                {
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ReasonPhrase = "Bad Request";
                }
            }
            else if (request.Method == HttpMethod.Get && path.Equals("cheeps", StringComparison.OrdinalIgnoreCase))
            {
                // support optional limit query
                var qs = System.Web.HttpUtility.ParseQueryString(request.RequestUri.Query);
                int? limit = null;
                if (int.TryParse(qs.Get("limit"), out var parsed))
                {
                    limit = parsed;
                }

                IEnumerable<ChirpProject.MainApp.Cheep> toReturn = _store;
                if (limit.HasValue)
                {
                    toReturn = _store.Take(limit.Value);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Content = JsonContent.Create(toReturn);
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = "Not Found";
            }

            return Task.FromResult(response);
        }
    }
}
