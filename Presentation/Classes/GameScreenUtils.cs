using Common;
using DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Classes
{

    public class GameScreenUtils
    {

        private static GameShipConfigRepository gameShipConfigRepository = new GameShipConfigRepository();

        public static GameScreen showPlayerShipConfig(Player player)
        {
            return new GameScreen(gameShipConfigRepository.GetGameShipConfigurationsByPlayerId(player.playerId).ToList());
        }

        public static bool isColliding(string fullCoordinate, Player player, GameScreen gameScreen)
        {
            bool isColliding = false;

            List<int[]> shipCellCoordinates = Utils.parseMultipleScreenCord(fullCoordinate);
            foreach (int[] shipCellCoordinate in shipCellCoordinates)
            {
                if (gameScreen.shipCellPresent(shipCellCoordinate)) isColliding = true;
            }

            return isColliding;
        }
    }
}
