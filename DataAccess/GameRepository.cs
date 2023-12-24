using Common;
using System.Linq;

namespace DataAccess
{
    public class GameRepository : ConnectionContext
    {
        public IQueryable<Game> GetGames()
        {
            var games = from game in Context.Games
                        select game;
            return games.AsQueryable();
        }

        public void addGame(Game game)
        {
            Context.Games.Add(game);
            Context.SaveChanges();
        }
    }
}
