using Common;
using DataAccess;
using Presentation.Classes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Presentation
{
    public class GameScreen
    {
        List<Cell> cells;
        public ShipRepository shipRepository;
        public GameScreen(List<Cell> cells)
        {
            this.cells = cells;
        }

        public GameScreen(List<Attack> attacks)
        {
            List<Cell> attackCells = new List<Cell>();
            foreach (Attack attack in attacks)
            {
                attackCells.Add(new AttackCell(attack.hit, Utils.parseSingleScreenCord(attack.coordinate)));
            }
            cells = attackCells;
        }

        public GameScreen(List<GameShipConfiguration> gameShipConfigurations)
        {
            List<Cell> shipCells = new List<Cell>();
            foreach (GameShipConfiguration gameShipConfiguration in gameShipConfigurations)
            {
                //gameShipConfig coordinates are 4 letters, representing both ends of a ship
                Ship ship = shipRepository.GetShipById(gameShipConfiguration.shipFK);
                List<int[]> shipBodyCoordinates = Utils.parseMultipleScreenCord(gameShipConfiguration.coordinate, ship.size);
                foreach (int[] point in shipBodyCoordinates) {
                    shipCells.Add(new ShipCell(point));
                }
            }

            cells = shipCells;
        }

        public void PrintGrid()
        {


            // Print the grid
            Console.WriteLine("  \t1\t2\t3\t4\t5\t6\t7\t8");
            for (int row = 0; row < 7; row++)
            {
                Console.WriteLine();
                Console.Write((char)('A' + row) + "\t");
                for (int col = 0; col < 8; col++)
                {
                    if (!specialCellPresent(cells, new int[] { row, col }))
                    {
                        Console.Write(String.Format("=", row, col));
                    }
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }

        private static bool specialCellPresent(List<Cell> cells, int[] position)
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
    }

    public abstract class Cell
    {
        public int[] coordinate { get; set; }
        public abstract void PrintCell();
    }

    public class ShipCell : Cell
    {

        public ShipCell(int[] coordinate)
        {
            this.coordinate = coordinate;
        }

        public override void PrintCell()
        {
            Console.Write("S");
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

    public class EmptyCell : Cell
    {
        public EmptyCell(int[] coordinate)
        {
            this.coordinate = coordinate;
        }

        public override void PrintCell()
        {
            Console.WriteLine("=");
        }
    }

}
