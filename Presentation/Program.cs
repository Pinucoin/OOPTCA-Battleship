﻿using Common;
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
        private static AttackRepository attackRepository = new AttackRepository();
        private static GameShipConfigRepository GameShipConfigRepository = new GameShipConfigRepository();
        private static GameScreen shipScreen = new GameScreen(new List<Cell>());
        private static GameScreen attackScreen = new GameScreen(new List<Cell>());
        private static Game currentGame = null;
        private static Player mainPlayer;
        private static Player otherPlayer;
        private static bool shipsSet = false;

        static void Main(string[] args)
        {

            string choice;

            do
            {
                Console.Clear();
                printMenu();
                choice = Console.ReadLine();
                try
                {
                    switch (choice)
                    {
                        case "1":
                            //Option 1: Add Player Details
                            if (currentGame == null)
                            {
                                addPlayersAndCreateGame();
                            }
                            else
                            {
                                Console.WriteLine("Game already running, unable to set up additional games/players.");
                            }
                            Console.ReadKey();
                            break;
                        case "2":
                            if (currentGame != null && !shipsSet)
                            {
                                //Option 2: Configure Ships
                                DisplayShipSetupUI();
                                shipsSet = true;
                            }
                            else if (currentGame != null && shipsSet)
                            {
                                Console.WriteLine(String.Format("Ships for current game titled [{0}] have already been set for both players. \nProceed to option 3.", currentGame.title));
                            }
                            else
                            {
                                Console.WriteLine("Set up a game/players before trying to configure ships.");
                            }
                            Console.ReadKey();
                            break;
                        case "3":
                            //Option 3: Launch Attack
                            if (currentGame != null && shipsSet)
                            {
                                displayGameBoards(mainPlayer, otherPlayer);
                                attack(mainPlayer, otherPlayer);
                                checkForWinner(mainPlayer);

                                if (!currentGame.complete)
                                {
                                    Player temp = mainPlayer;
                                    mainPlayer = otherPlayer;
                                    otherPlayer = temp;
                                }
                            }
                            else if (currentGame == null)
                            {
                                Console.WriteLine("Please set up players/game before trying to attack.");
                            }
                            else if (currentGame != null && !shipsSet)
                            {
                                Console.WriteLine("Please set up both players' ships before trying to attack.");
                            }
                            Console.ReadKey();
                            break;
                        case "4":
                            //Option 4: Quit
                            Console.WriteLine("Quitting...");
                            Console.ReadKey();
                            break;
                        default:
                            Console.WriteLine(String.Format("Invalid input [{0}]. Please try again. ", choice));
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An Error has occured.");
                    Console.ReadKey();
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

        private static void addPlayersAndCreateGame()
        {
            Game game = new Game();
            for (int i = 0; i < 2; i++)
            {
                bool validUsername = true;
                Player player = new Player();

                do
                {
                    validUsername = true;
                    Console.WriteLine("Input player username: ");
                    player.username = Console.ReadLine();

                    IQueryable<Player> players = playerRepository.GetPlayers().AsQueryable();
                    foreach (Player existingPlayer in players)
                    {
                        if (existingPlayer.username.ToUpper() == player.username.ToUpper())
                        {
                            Console.WriteLine(String.Format("Player with name [{0}] Already exists. Try again with a different username. ", existingPlayer.username));
                            validUsername = false;
                        }
                    }
                } while (!validUsername);

                Console.WriteLine("Input player password: ");
                player.password = getPassword();

                playerRepository.addPlayer(player);
                Console.WriteLine(String.Format("Player {0} created.", player.username));

                if (i == 0)
                {
                    game.creatorFK = player.playerId;
                    mainPlayer = player;
                }
                else
                {
                    game.opponentFK = player.playerId;
                    otherPlayer = player;
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
                        Console.Clear();

                        Console.WriteLine("======================================================================");
                        Console.WriteLine("   _____ ______ _______ _    _ _____  \r\n" +
                            "  / ____|  ____|__   __| |  | |  __ \\ \r\n" +
                            " | (___ | |__     | |  | |  | | |__) |\r\n" +
                            "  \\___ \\|  __|    | |  | |  | |  ___/ \r\n" +
                            "  ____) | |____   | |  | |__| | |     \r\n" +
                            " |_____/|______|  |_|   \\____/|_|     ");
                        Console.WriteLine("======================================================================");
                        Console.WriteLine("Ship setup for: " + player.username);
                        shipScreen = GameScreenUtils.showPlayerShipConfig(player);
                        shipScreen.PrintGrid();
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
                shipScreen = GameScreenUtils.showPlayerShipConfig(player);
                shipScreen.PrintGrid();
                Console.WriteLine("Press Any Key to Continue..");
                Console.ReadKey();

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
                if (!GameScreenUtils.isColliding(finalCoordinate, shipScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
            }
            //downward placement
            if ((arrayCoordinate[0] + ship.size - 1) < shipScreen.rows)
            {
                secondCoordinate = new int[] { arrayCoordinate[0] + (ship.size - 1), arrayCoordinate[1] };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, shipScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
            }
            //left placement
            if ((arrayCoordinate[1] - ship.size + 1) >= 0)
            {
                secondCoordinate = new int[] { arrayCoordinate[0], arrayCoordinate[1] - (ship.size - 1) };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, shipScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
            }
            //right placement
            if ((arrayCoordinate[1] + ship.size - 1) < shipScreen.cols)
            {
                secondCoordinate = new int[] { arrayCoordinate[0], arrayCoordinate[1] + (ship.size - 1) };
                string finalCoordinate = (startBoardCoordinate + Utils.arrayToBoardCoordinate(secondCoordinate));
                if (!GameScreenUtils.isColliding(finalCoordinate, shipScreen))
                {
                    validCoordinates.Add(finalCoordinate);
                }
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
                Console.ReadKey();
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
            bool validInput;
            do
            {
                validInput = true;
                boardCoordinate = Console.ReadLine();
                try
                {
                    isValidScreenCoordinate(boardCoordinate);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Invalid Coordinate")
                    {
                        Console.WriteLine("Invalid Coordinate.\nMake sure you are using the board coordinate notation (A1, B2 etc) and that it is within the visable board constraints. ");
                    }
                    validInput = false;
                }
            } while (!validInput);

            return boardCoordinate;
        }

        private static void isValidScreenCoordinate(string boardCoordinate)
        {
            int[] arrayCoordinate = Utils.parseSingleScreenCord(boardCoordinate);
            if (arrayCoordinate != null)
            {
                if (Utils.parseSingleScreenCord(boardCoordinate)[0] > shipScreen.rows - 1 || Utils.parseSingleScreenCord(boardCoordinate)[0] < 0 || Utils.parseSingleScreenCord(boardCoordinate)[1] > shipScreen.cols - 1 || Utils.parseSingleScreenCord(boardCoordinate)[1] < 0)
                {
                    throw new Exception("Invalid Coordinate");
                }
            }
            else {
                throw new Exception("Invalid Coordinate");
            }
        }

        private static void displayGameBoards(Player playerTurn, Player otherPlayer)
        {
            Console.Clear();
            Console.WriteLine("======================================================================");
            Console.WriteLine("        _______ _______       _____ _  __\r\n" +
                "     /\\|__   __|__   __|/\\   / ____| |/ /\r\n" +
                "    /  \\  | |     | |  /  \\ | |    | ' / \r\n" +
                "   / /\\ \\ | |     | | / /\\ \\| |    |  <  \r\n" +
                "  / ____ \\| |     | |/ ____ \\ |____| . \\ \r\n" +
                " /_/    \\_\\_|     |_/_/    \\_\\_____|_|\\_\\");
            Console.WriteLine("======================================================================");
            shipScreen = GameScreenUtils.showPlayerShipConfig(playerTurn, otherPlayer);
            attackScreen = GameScreenUtils.showPlayerAttackConfig(playerTurn);
            Console.WriteLine(String.Format("{0}'S SHIPS", playerTurn.username));
            shipScreen.PrintGrid();
            Console.WriteLine(String.Format("{0}'S ATTACKS", playerTurn.username));
            attackScreen.PrintGrid();
        }

        private static void attack(Player attacker, Player receiver)
        {
            bool validAttack = false;
            string attackCoordinate;
            bool isHit = false;
            do
            {
                bool skip = false;
                Console.WriteLine("Insert a valid coordinate to initiate your attack:");
                attackCoordinate = getAndValidateCoordinate();
                validAttack = GameScreenUtils.isValidAttackPosition(attackCoordinate, attackScreen);
                if (!validAttack)
                {
                    Console.WriteLine("Are you sure? You've already attack this cell. Proceeding will result in a failure. Y/N");
                    bool valid = false;
                    do
                    {
                        string input = Console.ReadLine().ToUpper();
                        switch (input)
                        {
                            case "Y":
                                Console.WriteLine("Invalid Attack Position. Past attack already present.\nTry again at a new position. ");
                                valid = true;
                                break;
                            case "N":
                                validAttack = false;
                                skip = true;
                                break;
                            default:
                                Console.WriteLine("Invalid Input. Make sure you are inputting Y or N.");
                                break;
                        }
                    } while (!valid && !skip);

                }

            } while (!validAttack);

            Console.WriteLine(String.Format("Sending Attack on Cell [{0}]", attackCoordinate));
            GameScreen enemySetup = GameScreenUtils.showPlayerShipConfig(receiver);
            isHit = GameScreenUtils.isHit(attackCoordinate, enemySetup);
            Attack attack = new Attack();
            attack.coordinate = attackCoordinate;
            attack.hit = isHit;
            attack.playerFK = attacker.playerId;
            attack.gameFK = currentGame.gameId;
            attackRepository.addAttack(attack);

            if (isHit)
            {
                Console.Write("Hit");
            }
            else {
                Console.Write("Miss");
            }
            Console.Write(String.Format(" on Cell [{0}]!\n", attackCoordinate));
        }

        private static void checkForWinner(Player player)
        {
            if (attackRepository.GetAttackHitsByPlayerId(player.playerId).Count() == 17)
            {
                gameRepository.concludeGameById(currentGame.gameId);
                currentGame.complete = true;
                Console.WriteLine(String.Format("Player {0} Wins!", player.username));
            }
        }
    }
}