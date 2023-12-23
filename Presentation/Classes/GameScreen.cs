using System.Collections.Generic;

namespace Presentation
{
    public class GameScreen
    {
        public GameScreen(List<Cell> cells)
        {
        }
    }

    public abstract class Cell
    {
        public abstract void PrintCell();
    }

    public class ShipCell : Cell
    {
    }

    public class AttackCell : Cell
    {

    }
}
