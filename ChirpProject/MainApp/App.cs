using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChirpProject.MainApp
{
    internal record Cheep
    {
        public string Author { get; set; }
        public string Message { get; set; }
        public string Timestamp { get; set; }
    }



    public class App
    {
        private readonly string csvDirectory;
        private readonly string csvFile;
        private readonly string header = "Author,Message,Timestamp";


        public App()
        {
            csvDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CHIRP", "resources");
            csvFile = Path.Combine(csvDirectory, "csv_db.csv");

            initialize();
        }


        public void StartProgram()
        {
            string helpMessage = @"
            Welcome to Cheep!
            Commands:
            * Cheep [message]: Send a cheep
            * Read: Read the cheeps
            * Help: Get this message again.
            * Exit: Exit the program
            ";

            Console.WriteLine(helpMessage);


            string[] input = new string[] { string.Empty };

            while (input[0] != "exit")
            {
                input = Console.ReadLine().ToLower().Split(' ');

                switch (input[0])
                {
                    case "cheep":
                        if (input.Length > 1)
                        {
                            SendCheep(input[1]);
                        }
                        break;
                    case "read":
                        ReadCheep();
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

        public void initialize()
        {
            if (!File.Exists(csvFile))
            {
                Directory.CreateDirectory(csvDirectory);
                writeToCheepDB(header);
            }
        }

        private void writeToCheepDB(string message)
        {
            using (StreamWriter sw = new StreamWriter(csvFile, append: true, Encoding.UTF8))
            {
                sw.WriteLine(message);
            }
        }

        public void SendCheep(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("No message detected. Please write your message like \"cheep [message]\"");
                return;
            }

            long unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            message = $"{Environment.UserName},\"{message}\",{unixTimestamp}";

            writeToCheepDB(message);
        }


        public void ReadCheep()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                Quote = '"',
                Mode = CsvMode.RFC4180,
                HasHeaderRecord = true
            };



            using (var reader = new StreamReader(csvFile))
            {
                using (var csv = new CsvReader(reader, config))
                {
                    IEnumerable<Cheep> records = csv.GetRecords<Cheep>();

                    foreach (Cheep record in records)
                    {
                        if (long.TryParse(record.Timestamp, out long result))
                        {
                            DateTime dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(result).UtcDateTime;

                            Console.WriteLine($"{record.Author} @ {dateTimeUtc:yyyy-MM-dd HH:mm:ss}: {record.Message}");
                        }


                    }
                }
            }

        }


    }
}
