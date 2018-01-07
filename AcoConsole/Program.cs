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
            var startTime = System.DateTime.Now;
            int[][] points = CreatePointsMatrix(10);

            var random = new Random();

            int[] startingPoint = points[0];

            int[] endPoint = points[1];

            var problem = new Problem(points, startingPoint, colonySize: 50, iterations: 100, endPoint: endPoint);
            ConsoleKey key;
            var it = 1;
            do
            {

                var rndStart = random.Next(points.Length - 1);

                int rndEnd;
                do
                {
                    rndEnd = random.Next(points.Length - 1);
                } while (rndEnd == rndStart);

                startingPoint = points[rndStart];

                endPoint = points[rndEnd];

                

                problem.SetStartingNode(startingPoint);
                problem.SetEndNode(endPoint);               

                var finalRoute = problem.FindRoute();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}a ejecucion", it.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine("Puntos");

                foreach (var point in points)
                {
                    Console.WriteLine("Lat: {0}, Long: {1}", point[0].ToString(), point[1].ToString());
                }

                Console.WriteLine();
                Console.WriteLine("Punto Inicial - Lat: {0}, Long: {1}", startingPoint[0].ToString(), startingPoint[1].ToString());
                Console.WriteLine("Punto Final - Lat: {0}, Long: {1}", endPoint[0].ToString(), endPoint[1].ToString());

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Camino final;");
                for (var i = 0; i < finalRoute.Length; i++)
                {
                    var point = finalRoute[i];
                    Console.WriteLine("Punto {0} - Lat: {1}, Long: {2}", i, point[0].ToString(), point[1].ToString());
                }

                var endTime = System.DateTime.Now;
                var difference = (endTime - startTime).TotalSeconds;


                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Inicio ejecucion: {0}", startTime.ToString());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Fin ejecucion: {0}", endTime.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Tiempo ejecucion: {0} segundos", difference.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;



                do
                {
                    Console.WriteLine("Volver a ejecutar? S/N?");
                    key = Console.ReadKey().Key;
                } while (key != ConsoleKey.S && key != ConsoleKey.N);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                it++;
            } while (key == ConsoleKey.S);
        }

        static int[][] CreatePointsMatrix(int pointCount)
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

            var points = new List<int[]>();

            var random = new Random();

            for (var i = 0; i < pointCount; i++)
            {
                var rndLat = random.Next(999);
                var rndLng = random.Next(999);
                int[] coord = { rndLat, rndLng };
                points.Add(coord);
            }

            //{ new int[] { 111, 223 },
            //                               new int[] { 2, 4 },
            //                               new int[] { 3333, 4561 },
            //                               new int[] { 44444, 23142 },
            //                               new int[] { 55, 87 } };


            var ret = points.ToArray();
           
            return ret;
        }



    }
}
