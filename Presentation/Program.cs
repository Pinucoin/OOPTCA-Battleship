using Common;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    public class Program
    {
        private static PlayerRepository playerRepository = new PlayerRepository();

        static void Main(string[] args)
        {
            string choice;

            do
            {
                Console.Clear();
                printMenu();
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        addPlayer();
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    default:
                        Console.WriteLine(String.Format("Invalid input [{0}]. Please try again. ", choice));
                        Console.ReadKey();
                        break;
                }

            } while (choice != "4");
        }

        public static void printMenu()
        {
            Console.WriteLine("======================================================================");
            Console.WriteLine("  ____       _______ _______ _      ______  _____ _    _ _____ _____  \r\n |  _ \\   /\\|__   __|__   __| |    |  ____|/ ____| |  | |_   _|  __ \\ \r\n | |_) | /  \\  | |     | |  | |    | |__  | (___ | |__| | | | | |__) |\r\n |  _ < / /\\ \\ | |     | |  | |    |  __|  \\___ \\|  __  | | | |  ___/ \r\n | |_) / ____ \\| |     | |  | |____| |____ ____) | |  | |_| |_| |     \r\n |____/_/    \\_\\_|     |_|  |______|______|_____/|_|  |_|_____|_| ");
            Console.WriteLine("======================================================================");
            Console.WriteLine("\n 1.\tAdd Player details\n 2.\tConfigure Ships\n 3.\tLaunch Attack\n 4.\tQuit\n");
        }

        public static void addPlayer()
        {
            bool validUsername = true;
            Player player = new Player();

            do {
                validUsername = true;
                Console.WriteLine("Input player username: ");
                player.username = Console.ReadLine();

                IQueryable<Player> players = playerRepository.GetPlayers().AsQueryable();
                foreach (Player existingPlayer in players) {
                    if (existingPlayer.username == player.username) {
                        Console.WriteLine(String.Format("Player with name [{0}] Already exists. Try again with a different username. ", existingPlayer.username));
                        validUsername = false;
                    }
                }
            }while(!validUsername);

                Console.WriteLine("Input player password: ");
            player.password = getPassword();

            playerRepository.addPlayer(player);
        }

        public static string getPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            do{
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key != ConsoleKey.Enter && keyInfo.Key != ConsoleKey.Escape)
                {
                    password += keyInfo.KeyChar;
                    Console.Write("*");
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }
    }
}
