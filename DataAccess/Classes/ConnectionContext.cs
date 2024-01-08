using Common;

namespace DataAccess
{
    public class ConnectionContext
    {
        public BattleshipDBEntities Context { get; set; }
        public ConnectionContext()
        {
            Context = new BattleshipDBEntities();
        }
    }
}
