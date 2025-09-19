using CsvHelper;
using CsvHelper.Configuration;
using System.Net.Http;
using System.Net.Http.Json;

namespace ChirpProject.MainApp
{
    internal record Cheep
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

        //Old code for previous test environment. Outcommented for testers.
        /*public App(bool test)
        {
            if (test == false) database = new CSVDatabase<Cheep>();
            else database = new TestDatabase<Cheep>();
        }*/

        public void StartProgram()
        {
            string helpMessage = @"
            Welcome to Cheep!
            Commands:
            * Cheep [message]: Send a cheep
            * Read: <limit (optional> Read the cheeps. If you give it a limit, it will give the first [limit] cheeps.
            * Help: Get this message again.
            * Exit: Exit the program
            ";

            Console.WriteLine(helpMessage);


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
                        Console.WriteLine(helpMessage);
                        break;
                    case "exit":
                        Console.WriteLine("Exiting the program. Goodbye (T_T)");
                        break;
                    default:
                        Console.WriteLine("The command was not recognized.");
                        Console.WriteLine(helpMessage);
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

        private async Task<IEnumerable<Cheep>> GetCheepAsyncJson(int? limit = null)
        {
            string URI = WebAPIUrl + "cheeps";
            if (limit != null) URI += "?limit=" + limit;

            HttpResponseMessage response = await client.GetAsync(URI);

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine(HandleResponse(response));
                return null;
            }

            IEnumerable<Cheep> cheeps = await response.Content.ReadFromJsonAsync<IEnumerable<Cheep>>();

            if(cheeps == null)
            {
                Console.WriteLine("Returned JSON from WebAPI is not in correct format.");
            }

            return cheeps;

        }


        public void SendCheep(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("No message detected. Please write your message like \"cheep [message]\"");
                return;
            }

            Cheep cheep = new Cheep(Environment.UserName, message, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            string result = PostCheepAsyncJson(cheep).GetAwaiter().GetResult();

            Console.WriteLine(result);
        }

        private async Task<string> PostCheepAsyncJson(Cheep cheep)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(WebAPIUrl + "cheep", cheep);
            return HandleResponse(response);
        }

        private string HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return response.ReasonPhrase;
            }
            else
            {
                return $"Error: {response.StatusCode}, {response.ReasonPhrase}";
            }
        }
    }
}