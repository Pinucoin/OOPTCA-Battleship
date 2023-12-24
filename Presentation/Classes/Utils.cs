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
            Console.WriteLine(String.Format("boardCoordinate: {0} {1} coordinate: {2} {3}", boardCoordinate[0], boardCoordinate[1], coordinate[0], coordinate[1]));
            return coordinate;
        }

        public static List<int[]> parseMultipleScreenCord(string multiCoordinate, int length)
        {
            int[] startCoordinate = new int[2];
            int[] endCoordinate = new int[2];
            List<int[]> completeCoordinate = new List<int[]>();

            startCoordinate[0] = multiCoordinate[0] - 'A';
            startCoordinate[1] = (multiCoordinate[1] - '0') - 1;
            endCoordinate[0] = multiCoordinate[2] - 'A';
            endCoordinate[1] = (multiCoordinate[3] - '0') - 1;

            if (startCoordinate[0] == endCoordinate[0])
            {
                if (startCoordinate[1] > endCoordinate[1])
                {
                    int[] temp = startCoordinate;
                    startCoordinate = endCoordinate;
                    endCoordinate = temp;
                }
                //Horizontal
                completeCoordinate = fillBetweenPoints(startCoordinate, length, true);
            }
            else if (startCoordinate[1] == endCoordinate[1])
            {
                if (startCoordinate[0] > endCoordinate[0])
                {
                    int[] temp = startCoordinate;
                    startCoordinate = endCoordinate;
                    endCoordinate = temp;
                }

                //Vertical
                completeCoordinate = fillBetweenPoints(startCoordinate, length, false);
            }

            return completeCoordinate;

        }

        private static List<int[]> fillBetweenPoints(int[] startPoint, int length, bool isHorizontal)
        {
            List<int[]> points = new List<int[]>();
            for (int i = 0; i < length; i++)
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
