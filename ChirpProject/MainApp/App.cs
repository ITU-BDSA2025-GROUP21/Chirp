using CsvHelper;
using CsvHelper.Configuration;
using SimpleDB;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChirpProject.MainApp.CheepClass;

namespace ChirpProject.MainApp
{
    public class App
    {
        public IDatabaseRepository<Cheep>database;

        public App()
        {
            database = new CSVDatabase<Cheep>();
        }

        public App(bool test)
        {
            if (test == false) database = new CSVDatabase<Cheep>();
            else database = new TestDatabase<Cheep>();
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
                        IterateCheeps();
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

        public void IterateCheeps()
        {
            IEnumerable<Cheep> cheeps = database.Read();

            foreach (Cheep cheep in cheeps)
            {
                DateTime dateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).UtcDateTime;

                Console.WriteLine($"{cheep.Author} @ {dateTimeUtc:yyyy-MM-dd HH:mm:ss}: {cheep.Message}");
            }
        }

        public void SendCheep(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine("No message detected. Please write your message like \"cheep [message]\"");
                return;
            }

            database.Store(new Cheep(message));
        }
    }
}