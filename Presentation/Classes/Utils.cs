using System;
using System.Collections.Generic;

namespace Presentation.Classes
{
    public class Utils
    {
        public static int[] parseSingleScreenCord(string boardCoordinate)
        {
            int[] coordinate = new int[2];
            coordinate[0] = boardCoordinate[0] - 'A';
            coordinate[1] = (boardCoordinate[1] - '0') - 1;
            return coordinate;
        }

        public static string arrayToBoardCoordinate(int[] arrayCoordinate)
        {
            string boardCoordinate = String.Format("{0}{1}", (char)(arrayCoordinate[0] + 'A'), (char)(arrayCoordinate[1] + 1 + '0'));
            return boardCoordinate;
        }

        public static List<int[]> parseMultipleScreenCord(string multiCoordinate)
        {
            int[] startCoordinate = new int[2];
            int[] endCoordinate = new int[2];
            int length;
            List<int[]> completeCoordinate = new List<int[]>();

            startCoordinate[0] = multiCoordinate[0] - 'A';
            startCoordinate[1] = (multiCoordinate[1] - '0') - 1;
            endCoordinate[0] = multiCoordinate[2] - 'A';
            endCoordinate[1] = (multiCoordinate[3] - '0') - 1;

            if (startCoordinate[0] == endCoordinate[0])
            {
                if (startCoordinate[1] > endCoordinate[1])
                {
                    //swap
                    int[] temp = startCoordinate;
                    startCoordinate = endCoordinate;
                    endCoordinate = temp;

                }
                //Horizontal
                length = endCoordinate[1] - startCoordinate[1];
                completeCoordinate = fillBetweenPoints(startCoordinate, length, true);
            }
            else if (startCoordinate[1] == endCoordinate[1])
            {
                if (startCoordinate[0] > endCoordinate[0])
                {
                    //swap
                    int[] temp = startCoordinate;
                    startCoordinate = endCoordinate;
                    endCoordinate = temp;
                }

                //Vertical
                length = endCoordinate[0] - startCoordinate[0];
                completeCoordinate = fillBetweenPoints(startCoordinate, length, false);
            }

            return completeCoordinate;

        }

        private static List<int[]> fillBetweenPoints(int[] startPoint, int length, bool isHorizontal)
        {
            List<int[]> points = new List<int[]>();
            for (int i = 0; i < length + 1; i++)
            {
                if (isHorizontal)
                {
                    points.Add(new int[] { startPoint[0], startPoint[1] + i });
                }
                else
                {
                    points.Add(new int[] { startPoint[0] + i, startPoint[1] });
                }
            }
            return points;
        }

        public static bool equateArrays(int[] array1, int[] array2)
        {
            if (array1[0] == array2[0] && array1[1] == array2[1])
            {
                return true;
            }
            return false;
        }

        public static string ConvertListToString(List<int[]> list)
        {
            return "[" + string.Join(", ", list.ConvertAll(arr => ArrayToString(arr))) + "]";
        }

        public static string ArrayToString(int[] arr)
        {
            return "[" + string.Join(", ", arr) + "]";
        }
    }
}
