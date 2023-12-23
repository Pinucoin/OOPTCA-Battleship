using Common;
using DataAccess;
using System;
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
                        //Option 1: Add Player Details
                        addPlayer();
                        break;
                    case "2":
                        //Option 2: Configure Ships
                        GameScreen.PrintGrid();
                        Console.ReadKey();
                        break;
                    case "3":
                        //Option 3: Launch Attack
                        break;
                    case "4":
                        //Option 4: Quit
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
            // printing menu UI
            Console.WriteLine("======================================================================");
            Console.WriteLine("  ____       _______ _______ _      ______  _____ _    _ _____ _____  \r\n |  _ \\   /\\|__   __|__   __| |    |  ____|/ ____| |  | |_   _|  __ \\ \r\n | |_) | /  \\  | |     | |  | |    | |__  | (___ | |__| | | | | |__) |\r\n |  _ < / /\\ \\ | |     | |  | |    |  __|  \\___ \\|  __  | | | |  ___/ \r\n | |_) / ____ \\| |     | |  | |____| |____ ____) | |  | |_| |_| |     \r\n |____/_/    \\_\\_|     |_|  |______|______|_____/|_|  |_|_____|_| ");
            Console.WriteLine("======================================================================");
            Console.WriteLine("\n 1.\tAdd Player Details\n 2.\tConfigure Ships\n 3.\tLaunch Attack\n 4.\tQuit\n");
        }

        public static void addPlayer()
        {
            bool validUsername = true;
            Player player = new Player();

            //Getting user input for username
            do
            {
                validUsername = true;
                Console.WriteLine("Input player username: ");
                player.username = Console.ReadLine();

                //Checking for existing players with the same username
                IQueryable<Player> players = playerRepository.GetPlayers().AsQueryable();
                foreach (Player existingPlayer in players)
                {
                    if (existingPlayer.username == player.username)
                    {
                        Console.WriteLine(String.Format("Player with name [{0}] Already exists. Try again with a different username. ", existingPlayer.username));
                        validUsername = false;
                    }
                }
            } while (!validUsername);

            //Getting user input for password
            Console.WriteLine("Input player password: ");
            player.password = getPassword();

            playerRepository.addPlayer(player);
        }

        public static string getPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            //Hidding user input with *
            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key != ConsoleKey.Enter)
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
