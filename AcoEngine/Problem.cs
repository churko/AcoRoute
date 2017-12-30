using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcoEngine
{
    public class Problem
    {
        Dictionary<int,Node> nodes;
        List<Arc> arcsInfo = new List<Arc>();
        int nnCount;
        Dictionary<int, List<int>> nearestNodes = new Dictionary<int, List<int>>();
        Node startingNode;
        Node endNode;
        int colonySize;
        int iterations; //termination condition
        BestSoFar bestSoFar;
        double initialPheromone;
        int routeLength;
        double heuristicsWeight;
        double qProbability;



        //constructor: initializes the problem parameters
        public Problem(int[][] points, int[] startingPoint, int colonySize = 30, int nnCount = 5,
            int iterations = 50, double heuristicsWeight = 2, double qProbability = 0.1, int[] endPoint = null)
        {
            //these should not be necessary since the application will pass the correct dimentions, however I'll leave them as placeholders
            if (!this.ValidatePoints(points))
            {
                //TODO devolver mensaje de error de dimension incorrecta de puntos de ruta
            }

            if (!this.ValidatePoints(startingPoint))
            {
                //TODO devolver mensaje de error de dimension incorrecta de punto inicial
            }

            if(endPoint != null && !this.ValidatePoints(endPoint))
            {
                //TODO devolver mensaje de error de dimension incorrecta de punto final
            }


            //initializes the number of iterations
            this.iterations = iterations;

            //Converts the points passed as arrays into nodes
            this.nodes = points.Select(x => new Node(x)).ToDictionary(x => x.NodeId, x => x);
            this.routeLength = this.nodes.Count;

            //initializes colony size
            this.colonySize = colonySize;

            //initializes the nearest neighbours list count
            this.nnCount = nnCount < this.routeLength  ? nnCount : this.routeLength - 1;

            //initializes the heuristicsWeight
            this.heuristicsWeight = heuristicsWeight;

            this.SetStartingNode(startingPoint);

            //builds arcs for the graph and the nearest neighbours list
            this.BuildGraph();

            //initializes the pheromones
            this.InitializePheromone();
        }

        public void SetStartingNode(int[] startingPoint)
        {
            if (this.nodes.Count() < 1)
            {
                //TODO devolver error de lista de nodos vacia
            }

            this.startingNode = nodes.Where(x => x.Value.Lat == startingPoint[0] && x.Value.Lng == startingPoint[1]).FirstOrDefault().Value;

            if (this.startingNode.NodeId < 1)
            {
                //TODO devolver error que el nodo elegido como inicial no existe
            }
        }

        private void BuildGraph()
        {
            foreach (var initNode in nodes)
            {
                var nearestNodes = new List<KeyValuePair<int, int>>();

                foreach (var endNode in nodes.Where(currNode => currNode.Value.NodeId != initNode.Value.NodeId ))
                {
                    //builds the arcs for the graph
                    var arcNodes = new Node[] { initNode.Value, endNode.Value };
                    //var arcNodesIds = new int[2] { initNode.Key, endNode.Key };
                    var arc = new Arc(arcNodes, this.initialPheromone, this.heuristicsWeight);
                    this.arcsInfo.Add(arc);

                    //adds the node and distance to search for the nearest neighbours of initNode
                    nearestNodes.Add(new KeyValuePair<int, int>(endNode.Key, arc.Distance));
                }

                //get the (nnCount)nth nodes with the shortest distance to initNodes
                nearestNodes.Sort((x, y) => x.Value.CompareTo(y.Value));
                nearestNodes.Take(this.nnCount);

                //builds the nearestNodes dictionary
                var nearestNeighbours = new List<int>();
                nearestNeighbours = Enumerable.Range(0, this.nnCount).Select(x => nearestNodes[x].Key).ToList();
                this.nearestNodes.Add(initNode.Key, nearestNeighbours);
            }
        }

        private void InitializePheromone()
        {
            var initialNodes = this.nodes.Select(x => x.Value).ToArray();

            var tour = new List<Node>();
            var nnDistance = 0;

            var nextNode = this.startingNode; 
            tour.Add(nextNode);
            for (var i = 1; i < this.routeLength; i++)
            {
                var pairs = this.nodes.Where(currNode => !tour.Any(tNode => tNode.NodeId == currNode.Key)).Select(currNode => new Node[] { nextNode, currNode.Value }).ToList();
                var nodeArcs = this.arcsInfo.Where(x => pairs.Any(pNode => pNode[0].NodeId == x.InitNodeId && pNode[1].NodeId == x.EndNodeId)).OrderBy(o => o.Distance).ToList();
                nextNode = this.nodes[nodeArcs[0].EndNodeId];
                nnDistance += nodeArcs[0].Distance;
                tour.Add(nextNode);
            }
            nnDistance += this.arcsInfo.Where(x => x.InitNodeId == nextNode.NodeId && x.EndNodeId == this.startingNode.NodeId).FirstOrDefault().Distance;

            this.initialPheromone = Convert.ToDouble(1) / Convert.ToDouble(this.routeLength * nnDistance);

        }

        //starts the search
        public int[][] FindRoute()
        {
            for (var i = 0; i < this.iterations; i++)
            {
                this.bestSoFar = this.ContructSolutions();
                this.bestSoFar = this.TwoOpt();
                this.UpdateStats(i);
                this.UpdatePheromone();
            }

            int[][] finalRoute = new int[1][] { new int[2] { 2, 4 } };
            return finalRoute;
        }

        private BestSoFar ContructSolutions()
        {
            List<Ant> antColony = Enumerable.Range(0, this.colonySize).Select(x => new Ant(this.nodes)).ToList();

            for (var i = 0; i < this.nodes.Count; i++)
            {
                foreach (var ant in antColony)
                {
                    //ant.FindNextNode(this.arcsInfo, this.nearestNodes);
                    //TODO termine aca
                }
            }

            var iterationBest = new BestSoFar();
            return iterationBest.Distance < this.bestSoFar.Distance ? iterationBest : this.bestSoFar;
        }

        private BestSoFar TwoOpt()
        {
            var localBest = new BestSoFar();
            return localBest.Distance < this.bestSoFar.Distance ? localBest : this.bestSoFar;
        }

        private void UpdateStats(int iteration)
        {
            this.bestSoFar.Iteration = iteration;
        }

        private void UpdatePheromone()
        {
            throw new NotImplementedException();
        }

        #region Useful but probably unused
        public static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        private bool ValidatePoints(int[] point)
        {
            if(point.GetLength(0) != 2)
            {
                return false;
            }
            return true;
        }

        private bool ValidatePoints(int[][] points)
        {
            foreach(var point in points)
            {
                if(!this.ValidatePoints(point))
                {
                    return false;
                }
            }
            return true;
        }

        //public void AddNode(int[])
        #endregion
    }

    internal class BestSoFar
    {
        public List<Node> Route { get; set; }
        public int Distance { get; set; }
        public int Iteration { get; set; }

    }
}
