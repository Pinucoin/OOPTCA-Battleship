using Common;
using System.Linq;

namespace DataAccess
{
    public class PlayerRepository : ConnectionContext
    {

        public IQueryable<Player> GetPlayers()
        {
            //Getting all players in player table
            var players = from player in Context.Players
                          select player;
            return players.AsQueryable();
        }

        public void addPlayer(Player player)
        {
            //Adding player and saving changes
            Context.Players.Add(player);
            Context.SaveChanges();
        }
    }
}
