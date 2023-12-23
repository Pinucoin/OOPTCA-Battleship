using System;
using System.Collections.Generic;

namespace Presentation
{
    public class GameScreen
    {
        public GameScreen(List<Cell> cells)
        {
        }

        public static void PrintGrid()
        {
            char[,] grid = new char[7, 8];

            // Fill the grid with dots
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    grid[row, col] = '=';
                }
            }

            // Print the grid
            Console.WriteLine("  \t1\t2\t3\t4\t5\t6\t7\t8");
            for (int row = 0; row < 7; row++)
            {
                Console.WriteLine();
                Console.Write((char)('A' + row) + "\t"); 
                for (int col = 0; col < 8; col++)
                {
                    Console.Write(grid[row, col] + "\t"); 
                }
                Console.WriteLine();
            }
        }
    }

    public abstract class Cell
    {
        //public abstract void PrintCell();
    }

    public class ShipCell : Cell
    {
    }

    public class AttackCell : Cell
    {

    }
}
