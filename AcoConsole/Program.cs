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
            System.DateTime startTime;
            double[][] points = CreatePointsMatrix(50);

            var random = new Random();

            double[] startingPoint = points[0];

            double[] endPoint = points[1];

            var problem = new Problem(points, startingPoint, colonySize: 50, iterations: 10, endPoint: endPoint);
            ConsoleKey key;
            var it = 1;
            do
            {
                startTime = System.DateTime.Now;
                var rndStart = random.Next(points.Length - 1);

                int rndEnd;
                do
                {
                    rndEnd = random.Next(points.Length - 1);
                } while (rndEnd == rndStart);

                startingPoint = points[2];

                endPoint = points[0];

                

                problem.SetStartingNode(startingPoint);
                problem.SetEndNode(endPoint);               

                var finalRoute = problem.FindRoute();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0}a ejecucion", it.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine("Puntos");

                foreach (var point in points)
                {
                    Console.WriteLine("PersonId: {2} - Lat: {0}, Long: {1}", point[0].ToString(), point[1].ToString(), point[2].ToString());
                }

                Console.WriteLine();
                Console.WriteLine("PersonId: {2} - Punto Inicial - Lat: {0}, Long: {1}", startingPoint[0].ToString(), startingPoint[1].ToString(), startingPoint[2].ToString());
                Console.WriteLine("PersonId: {2} - Punto Final - Lat: {0}, Long: {1}", endPoint[0].ToString(), endPoint[1].ToString(), endPoint[2].ToString());

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Camino final;");
                for (var i = 0; i < finalRoute.Length; i++)
                {
                    var point = finalRoute[i];
                    Console.WriteLine("Punto {0} - PersonId: {3} - Lat: {1}, Long: {2}", i, point[0].ToString(), point[1].ToString(), point[2].ToString());
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

        static double[][] CreatePointsMatrix(int pointCount)
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

            //var points = new List<int[]>();

            //var random = new Random();

            //for (var i = 0; i < pointCount; i++)
            //{
            //    var rndLat = random.Next(999);
            //    var rndLng = random.Next(999);
            //    int[] coord = { rndLat, rndLng };
            //    points.Add(coord);
            //}

            var points = new List<double[]> {
                                            new double[] { -6.21, -5.1234123412,1 },
                                           new double[] {-8.1234123, -16.345623122,2 },
                                           new double[] { -10.4433229890, -8.12341234111,3 },
                                           new double[] { -14.12341234111, -25.9767867890,4 },
                                           new double[] { -22.1234123111, -14.12341234444,5 } };


            var ret = points.ToArray();
           
            return ret;
        }



    }
}
