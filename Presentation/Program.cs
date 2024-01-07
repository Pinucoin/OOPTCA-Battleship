using Common;
using DataAccess;
using Presentation.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Presentation
{
    public class Program
    {
        private static PlayerRepository playerRepository = new PlayerRepository();
        private static GameRepository gameRepository = new GameRepository();
        private static ShipRepository shipRepository = new ShipRepository();
        private static GameShipConfigRepository GameShipConfigRepository = new GameShipConfigRepository();
        private static GameScreen gameScreen = new GameScreen(new List<Cell>());
        private static Game currentGame;

        static void Main(string[] args)
        {
            string choice;

            do
            {
                Console.Clear();
                printMenu();
                Console.WriteLine(Utils.ConvertListToString(Utils.parseMultipleScreenCord("F3D3")));
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        //Option 1: Add Player Details
                        addPlayersCreateGame();
                        Console.ReadKey();
                        break;
                    case "2":
                        //Option 2: Configure Ships
                        DisplayShipSetupUI();
                        Console.ReadKey();
                        break;
                    case "3":
                        //Option 3: Launch Attack
                        Console.ReadKey();
                        break;
                    case "4":
                        //Option 4: Quit
                        Console.ReadKey();
                        break;
                    default:
                        Console.WriteLine(String.Format("Invalid input [{0}]. Please try again. ", choice));
                        Console.ReadKey();
                        break;
                }

            } while (choice != "4");
        }

        private static void printMenu()
        {
            // printing menu UI
            Console.WriteLine("======================================================================");
            Console.WriteLine("  ____       _______ _______ _      ______  _____ _    _ _____ _____  \r\n" +
                " |  _ \\   /\\|__   __|__   __| |    |  ____|/ ____| |  | |_   _|  __ \\ \r\n" +
                " | |_) | /  \\  | |     | |  | |    | |__  | (___ | |__| | | | | |__) |\r\n" +
                " |  _ < / /\\ \\ | |     | |  | |    |  __|  \\___ \\|  __  | | | |  ___/ \r\n" +
                " | |_) / ____ \\| |     | |  | |____| |____ ____) | |  | |_| |_| |     \r\n" +
                " |____/_/    \\_\\_|     |_|  |______|______|_____/|_|  |_|_____|_| ");
            Console.WriteLine("======================================================================");
            Console.WriteLine("\n 1.\tAdd Player Details\n 2.\tConfigure Ships\n 3.\tLaunch Attack\n 4.\tQuit\n");
        }

        private static void addPlayersCreateGame()
        {
            Game game = new Game();
            for (int i = 0; i < 2; i++)
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
                Console.WriteLine(String.Format("Player {0} created.", player.username));

                if (i == 0)
                {
                    game.creatorFK = player.playerId;
                }
                else
                {
                    game.opponentFK = player.playerId;
                }
            }

            Console.WriteLine("2 Players Added; Insert Game Title:");
            game.title = Console.ReadLine();
            game.complete = false;

            gameRepository.addGame(game);
            currentGame = game;
            Console.WriteLine(String.Format("Game with Title {0} added. ", game.title));
        }

        private static string getPassword()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            //Hiding user input with *
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

        private static void DisplayShipSetupUI()
        {
            List<Player> players = new List<Player>();
            players.Add(playerRepository.GetPlayerById(currentGame.creatorFK));
            players.Add(playerRepository.GetPlayerById(currentGame.opponentFK));

            foreach (Player player in players)
            {

                IQueryable<Ship> availableShips = shipRepository.GetShips();

                Console.WriteLine("======================================================================");
                Console.WriteLine("   _____ ______ _______ _    _ _____  \r\n" +
                    "  / ____|  ____|__   __| |  | |  __ \\ \r\n" +
                    " | (___ | |__     | |  | |  | | |__) |\r\n" +
                    "  \\___ \\|  __|    | |  | |  | |  ___/ \r\n" +
                    "  ____) | |____   | |  | |__| | |     \r\n" +
                    " |_____/|______|  |_|   \\____/|_|     ");
                Console.WriteLine("======================================================================");
                Console.WriteLine(String.Format("{0} VS {1}", players[0].username, players[1].username));

                SetupShips(player);
            }
        }

        private static void SetupShips(Player player)
        {
            List<Ship> availableShips = shipRepository.GetShips().ToList();

            do
            {
                Ship selectedShip = null;
                string startBoardCoordinate;
                string fullCoordinate;
                int selection;

                do
                {
                    do
                    {

                        Console.WriteLine("Ship setup for: " + player.username);
                        gameScreen = GameScreenUtils.showPlayerShipConfig(player);
                        gameScreen.PrintGrid();
                        Console.WriteLine("Select ship to set up by id.");
                        printShipSelection(availableShips);
                        selection = Convert.ToInt32(Console.ReadLine());

                        foreach (Ship ship in availableShips)
                        {
                            if (ship.shipId == selection) selectedShip = ship;
                        }

                        if (selectedShip == null) Console.WriteLine("Invalid Ship Id. try again.");

                    } while (selectedShip == null);

                    Console.WriteLine("Input a coordinate for the first end of the ship (ex: A1): ");
                    startBoardCoordinate = getAndValidateCoordinate();
                    fullCoordinate = getFullCoordinateFromUser(startBoardCoordinate, player, selectedShip);
                } while (fullCoordinate == "");
                availableShips.Remove(selectedShip);

                GameShipConfiguration gameShipConfiguration = new GameShipConfiguration();
                gameShipConfiguration.playerFK = player.playerId;
                gameShipConfiguration.gameFK = currentGame.gameId;
                gameShipConfiguration.shipFK = selectedShip.shipId;
                gameShipConfiguration.coordinate = fullCoordinate;

                GameShipConfigRepository.addGameShipConfiguration(gameShipConfiguration);
                gameScreen = GameScreenUtils.showPlayerShipConfig(player);
                gameScreen.PrintGrid();

            } while (availableShips.Count() > 0);
        }

        private static string getFullCoordinateFromUser(string startBoardCoordinate, Player player, Ship ship)
        {
            int[] arrayCoordinate = Utils.parseSingleScreenCord(startBoardCoordinate);
            int[] secondCoordinate;
            List<String> validCoordinates = new List<string>();
            bool validSelection = false;
            string chosenFullCoordinate = "";

            //upward placement
            if ((arrayCoordinate[0] - ship.size + 1) >= 0)
            {
                secondCoordinate = new int[] { arrayCoordinate[0] - (ship.size - 1), arrayCoordinate[1] };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, player, gameScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
                else Console.WriteLine("Found ship.");
            }
            //downward placement
            if ((arrayCoordinate[0] + ship.size - 1) < gameScreen.rows)
            {
                secondCoordinate = new int[] { arrayCoordinate[0] + (ship.size - 1), arrayCoordinate[1] };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, player, gameScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
                else Console.WriteLine("Found ship.");
            }
            //left placement
            if ((arrayCoordinate[1] - ship.size + 1) >= 0)
            {
                secondCoordinate = new int[] { arrayCoordinate[0], arrayCoordinate[1] - (ship.size - 1) };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, player, gameScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
                else Console.WriteLine("Found ship.");
            }
            //right placement
            if ((arrayCoordinate[1] + ship.size - 1) < gameScreen.cols)
            {
                secondCoordinate = new int[] { arrayCoordinate[0], arrayCoordinate[1] + (ship.size - 1) };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, player, gameScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
                else Console.WriteLine("Found ship.");
            }

            if (validCoordinates.Count() > 0)
            {

                string[] validCoordinatesArray = validCoordinates.ToArray();

                Console.WriteLine("Select a placement: ");
                //display all valid placements
                for (int i = 0; i < validCoordinatesArray.Length; i++)
                {
                    Console.WriteLine(String.Format("[{0}] {1}", i + 1, validCoordinatesArray[i]));
                }

                do
                {
                    int placementNumber = Convert.ToInt32(Console.ReadLine());
                    if (placementNumber > 0 && placementNumber <= validCoordinatesArray.Length)
                    {
                        validSelection = true;
                        chosenFullCoordinate = validCoordinatesArray[placementNumber - 1];
                    }
                    else Console.WriteLine("Invalid Selection. Make sure you are selection a valid placement number (value in the [ ])");

                } while (!validSelection);

                return chosenFullCoordinate;
            }
            else
            {
                Console.WriteLine(String.Format("No valid placement for {0} with size {1}. try again at a new coordinate.", ship.title, ship.size));
                return "";
            }
        }

        private static void printShipSelection(List<Ship> availableShips)
        {
            Console.WriteLine("----- SHIPS ------\nID\tSize\tTitle");
            foreach (Ship ship in availableShips)
            {
                Console.WriteLine(String.Format("{0}\t{1}\t{2}", ship.shipId, ship.size, ship.title));
            }
        }

        private static string getAndValidateCoordinate()
        {
            string boardCoordinate;
            do
            {
                boardCoordinate = Console.ReadLine();
                if (!isValidScreenCoordinate(boardCoordinate))
                {
                    Console.WriteLine("Invalid Coordinate.\nMake sure you are using the board coordinate notation (A1, B2 etc) and that it is within the visable board constraints. ");
                }
            } while (!isValidScreenCoordinate(boardCoordinate));

            return boardCoordinate;
        }

        private static bool isValidScreenCoordinate(string boardCoordinate)
        {
            Console.WriteLine(boardCoordinate);
            if (Utils.parseSingleScreenCord(boardCoordinate)[0] > gameScreen.rows-1 || Utils.parseSingleScreenCord(boardCoordinate)[0] < 0 || Utils.parseSingleScreenCord(boardCoordinate)[1] > gameScreen.cols-1 || Utils.parseSingleScreenCord(boardCoordinate)[1] < 0) return false;
            return true;
        }


    }
}
