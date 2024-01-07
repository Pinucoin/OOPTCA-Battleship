using Common;
using DataAccess;
using Presentation.Classes;
using System;
using System.Collections.Generic;

namespace Presentation
{
    public class GameScreen
    {
        List<Cell> cells;
        public int rows { get; }
        public int cols { get; }
        public ShipRepository shipRepository = new ShipRepository();
        public GameScreen(List<Cell> cells)
        {
            this.cells = cells;
            rows = 7;
            cols = 8;
        }

        public GameScreen(List<Attack> attacks)
        {
            rows = 7;
            cols = 8;
            List<Cell> attackCells = new List<Cell>();
            foreach (Attack attack in attacks)
            {
                attackCells.Add(new AttackCell(attack.hit, Utils.parseSingleScreenCord(attack.coordinate)));
            }
            cells = attackCells;
        }

        public GameScreen(List<GameShipConfiguration> gameShipConfigurations)
        {
            rows = 7;
            cols = 8;
            List<Cell> shipCells = new List<Cell>();
            foreach (GameShipConfiguration gameShipConfiguration in gameShipConfigurations)
            {
                Ship ship = shipRepository.GetShipById(gameShipConfiguration.shipFK);
                //gameShipConfig coordinates are 4 letters, representing both ends of a ship
                List<int[]> shipBodyCoordinates = Utils.parseMultipleScreenCord(gameShipConfiguration.coordinate);
                Console.WriteLine(Utils.ConvertListToString(shipBodyCoordinates));
                foreach (int[] point in shipBodyCoordinates)
                {
                    shipCells.Add(new ShipCell(point, ship.title));
                }
            }

            cells = shipCells;
        }

        public void PrintGrid()
        {
            Console.WriteLine(cells.Count);
            // Print the grid
            Console.WriteLine("  \t1\t2\t3\t4\t5\t6\t7\t8");
            for (int row = 0; row < rows; row++)
            {
                Console.WriteLine();
                Console.Write((char)('A' + row) + "\t");
                for (int col = 0; col < cols; col++)
                {
                    if (!specialCellPresent(new int[] { row, col }))
                    {
                        Console.Write(String.Format("≈", row, col));
                    }
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }

        public bool specialCellPresent(int[] position)
        {
            bool present = false;
            foreach (Cell cell in cells)
            {
                if (Utils.equateArrays(cell.coordinate, position))
                {

                    cell.PrintCell();
                    present = true;
                }
            }
            return present;
        }

        public bool shipCellPresent(int[] position)
        {
            bool present = false;
            foreach (Cell cell in cells)
            {
                if (cell is ShipCell && Utils.equateArrays(cell.coordinate, position))
                {
                    present = true;
                }
            }
            return present;
        }
    }

    public abstract class Cell
    {
        public int[] coordinate { get; set; }
        public abstract void PrintCell();
    }

    public class ShipCell : Cell
    {
        string title;

        public ShipCell(int[] coordinate, string title)
        {
            this.coordinate = coordinate;
            this.title = title.Substring(0,2).ToUpper();
        }

        public override void PrintCell()
        {
            Console.Write(title);
        }
    }

    public class AttackCell : Cell
    {
        public bool isHit { get; }

        public AttackCell(bool isHit, int[] coordinate)
        {
            this.isHit = isHit;
            this.coordinate = coordinate;
        }

        public bool getHit()
        {
            return isHit;
        }

        public override void PrintCell()
        {
            if (isHit) Console.Write("X");
            else Console.Write("O");
        }
    }


}
