using AcoEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcoConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            int[][] points = CreatePointsMatrix();

            int[] startingPoint = points[0];

            var problem = new Problem(points, startingPoint, colonySize: 1, iterations: 1);

            problem.FindRoute();

            Console.WriteLine("enter to exit");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        static int[][] CreatePointsMatrix()
        {
            //Console.WriteLine("input number of points");
            //int intTemp = Convert.ToInt32(Console.ReadLine());

            //List<int[]> points =new List<int[]>();
            //int x;
            //int y;

            //for (var i = 0; i < intTemp; i++)
            //{
            //    Console.WriteLine("X" + i.ToString());

            //    x = Convert.ToInt32(Console.ReadLine()); 

            //    Console.WriteLine("Y"+ i.ToString());

            //    y = Convert.ToInt32(Console.ReadLine());

            //    int[] point = { x, y };

            //    points.Add(point);
            //}

            var points = new List<int[]> { new int[] { 111, 223 },
                                           new int[] { 2, 4 },
                                           new int[] { 3333, 4561 },
                                           new int[] { 44444, 23142 },
                                           new int[] { 55, 87 } };


            var ret = points.ToArray();
           
            return ret;
        }



    }
}
