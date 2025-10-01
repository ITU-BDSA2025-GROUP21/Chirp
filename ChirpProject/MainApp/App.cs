using CsvHelper;
using CsvHelper.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChirpProject.MainApp
{
    //Testing
    public record Cheep
    {
        public Cheep(string Author, string Message, long Timestamp)
        {
            this.Message = Message;
            this.Author = Author;
            this.Timestamp = Timestamp;
        }

        public string Author { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }
    public class App
    {
        private HttpClient client;
        private string WebAPIUrl = "https://bdsagroup21chirpremotedb.azurewebsites.net/";
        public App()
        {
            client = new HttpClient();
        }

        // Constructor overload for dependency injection (tests can inject a fake HttpClient)
        public App(HttpClient httpClient, string webApiUrl = null)
        {
            client = httpClient ?? new HttpClient();
            if (!string.IsNullOrEmpty(webApiUrl)) WebAPIUrl = webApiUrl;
        }

   

        public void StartProgram()
        {
            UserInterface.sendHelpMessage();

            string[] input = new string[] { string.Empty };

            while (input[0] != "exit")
            {
                input = Console.ReadLine().Split(' ', 2);
                input[0] = input[0].ToLower().Trim();

                switch (input[0])
                {
                    case "cheep":
                        if (input.Length > 1)
                        {
                            SendCheep(input[1]);
                        }
                        break;
                    case "read":
                        int? limit = null;
                        if (input.Length > 1 && int.TryParse(input[1], out int parsed))
                        {
                            limit = parsed;
                        }
                        IterateCheeps(limit);
                        break;
                    case "help":
                        UserInterface.sendHelpMessage();
                        break;
                    case "exit":
                        UserInterface.sendExitMessage();
                        break;
                    default:
                        UserInterface.sendDefaultMessage();
                        UserInterface.sendHelpMessage();
                        break;
                }
            }
        }

        public async void IterateCheeps(int? limit = null)
        {

            IEnumerable<Cheep> cheeps = await GetCheepAsyncJson(limit);

            foreach (Cheep cheep in cheeps)
            {
                DateTime dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).UtcDateTime;

                Console.WriteLine($"{cheep.Author} @ {dateTimeUtc:yyyy-MM-dd HH:mm:ss}: {cheep.Message}");
            }
        }

        // MADE PUBLIC and return List<Cheep> (keeps async). Tests will call this.
        public async Task<List<Cheep>> GetCheepAsyncJson(int? limit = null)
        {
            string URI = WebAPIUrl + "cheeps";
            if (limit != null) URI += "?limit=" + limit;

            HttpResponseMessage response = await client.GetAsync(URI);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(HandleResponse(response));
                return new List<Cheep>();
            }

            IEnumerable<Cheep> cheeps = await response.Content.ReadFromJsonAsync<IEnumerable<Cheep>>();

            if (cheeps == null)
            {
                Console.WriteLine("Returned JSON from WebAPI is not in correct format.");
                return new List<Cheep>();
            }

            return cheeps.ToList();
        }


        public void SendCheep(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                UserInterface.sendCheepErrorMessage();
                return;
            }

            database.Store(new Cheep(message));
            UserInterface.sendCheepMessage();
        }
    }
}
