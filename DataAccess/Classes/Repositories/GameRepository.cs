using Common;
using System.Linq;

namespace DataAccess
{
    public class GameRepository : ConnectionContext
    {

        public void addGame(Game game)
        {
            Context.Games.Add(game);
            Context.SaveChanges();
        }

        public Game getGameById(int id)
        {
            var selectedGame = from game in Context.Games
                               where game.gameId == id
                               select game;
            return selectedGame.FirstOrDefault();
        }

        public void concludeGameById(int gameId)
        {

            var originalGame = getGameById(gameId);
            originalGame.complete = true;
            Context.SaveChanges();

        }
    }
}
