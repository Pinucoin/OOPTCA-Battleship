using Common;
using System.Linq;

namespace DataAccess
{
    public class ShipRepository : ConnectionContext
    {
        public Ship GetShipById(int id)
        {
            var selectedShip = from ship in Context.Ships
                               where ship.shipId == id
                               select ship;
            return selectedShip.FirstOrDefault();
        }

        public IQueryable<Ship> GetShips()
        {
            var ships = from ship in Context.Ships
                        select ship;
            return ships.AsQueryable();
        }

    }
}
