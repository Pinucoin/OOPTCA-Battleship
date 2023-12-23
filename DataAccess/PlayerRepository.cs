using Common;
using System.Linq;

namespace DataAccess
{
    public class PlayerRepository : ConnectionContext
    {
        public PlayerRepository() : base()
        {

        }

        public IQueryable<Player> GetPlayers()
        {
            var players = from player in Context.Players
                          select player;
            return players.AsQueryable();
        }

        public void addPlayer(Player player)
        {
            Context.Players.Add(player);
            Context.SaveChanges();
        }
    }
}
