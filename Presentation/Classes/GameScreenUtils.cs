using Common;
using DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace Presentation.Classes
{
    public class GameScreenUtils
    {
        private static GameShipConfigRepository gameShipConfigRepository = new GameShipConfigRepository();
        private static AttackRepository attackRepository = new AttackRepository();

        public static GameScreen showPlayerShipConfig(Player player)
        {
            return new GameScreen(gameShipConfigRepository.GetGameShipConfigurationsByPlayerId(player.playerId).ToList());
        }

        public static GameScreen showPlayerShipConfig(Player mainPlayer, Player opponent)
        {
            return new GameScreen(gameShipConfigRepository.GetGameShipConfigurationsByPlayerId(mainPlayer.playerId).ToList(),
                attackRepository.GetAttackHitsByPlayerId(opponent.playerId).ToList());
        }

        public static GameScreen showPlayerAttackConfig(Player player)
        {
            return new GameScreen(attackRepository.GetAttacksByPlayerId(player.playerId).ToList());
        }

        public static bool isValidAttackPosition(string coordinate, GameScreen gameScreen)
        {
            bool isValid = true;
            int[] arrayCoordinate = Utils.parseSingleScreenCord(coordinate);
            if (gameScreen.isAttackCellPresent(arrayCoordinate))
            {
                isValid = false;
            }
            return isValid;
        }

        public static bool isHit(string coordinate, GameScreen gameScreen)
        {
            bool isHit = false;

            int[] attackCellCoordinate = Utils.parseSingleScreenCord(coordinate);

            if (gameScreen.isShipCellPresent(attackCellCoordinate)) isHit = true;

            return isHit;
        }

        public static bool isColliding(string fullCoordinate, GameScreen gameScreen)
        {
            bool isColliding = false;

            List<int[]> shipCellCoordinates = Utils.parseMultipleScreenCord(fullCoordinate);
            foreach (int[] shipCellCoordinate in shipCellCoordinates)
            {
                if (gameScreen.isShipCellPresent(shipCellCoordinate)) isColliding = true;
            }

            return isColliding;
        }
    }
}
