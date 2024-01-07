using Common;
using System.Linq;

namespace DataAccess
{
    public class GameShipConfigRepository : ConnectionContext
    {
        public IQueryable<GameShipConfiguration> GetGameShipConfigurations()
        {
            var gameShipConfigurationList = from gameShipConfiguration in Context.GameShipConfigurations
                                            select gameShipConfiguration;
            return gameShipConfigurationList.AsQueryable();
        }

        public IQueryable<GameShipConfiguration> GetGameShipConfigurationsByPlayerId(int playerId)
        {
            var gameShipConfigurationList = from gameShipConfiguration in Context.GameShipConfigurations
                                            where gameShipConfiguration.playerFK == playerId
                                            select gameShipConfiguration;
            return gameShipConfigurationList.AsQueryable();
        }

        public void addGameShipConfiguration(GameShipConfiguration gameShipConfiguration)
        {
            Context.GameShipConfigurations.Add(gameShipConfiguration);
            Context.SaveChanges();
        }
    }
}
