using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChirpProject.MainApp
{
    public class UserInterface
    {
        public static void sendHelpMessage()
        {
            string helpMessage = @"
            Welcome to Chirp!
            Commands:
            * Cheep [message]: Send a cheep
            * Read: Read the cheeps
            * Help: Get this message again.
            * Exit: Exit the program
            ";
            Console.WriteLine(helpMessage);
        }

        public static void sendExitMessage()
        {
            Console.WriteLine("Exiting the program. Goodbye (T_T)");
        }

        public static void sendDefaultMessage()
        {
            Console.WriteLine("The command was not recognized. Type 'help' for a list of commands.");
        }

        public static void sendCheepMessage()
        {
            Console.WriteLine("Cheep sent!");
        }

        public static void sendCheepErrorMessage()
        {
            Console.WriteLine("Error: No message provided. Usage: cheep [message]");
        }
    }
}
