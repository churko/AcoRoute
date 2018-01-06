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
        int? endNodeId;
        int colonySize;
        int iterations; //termination condition
        BestSoFar bestSoFar = new BestSoFar();
        double initialPheromone;
        double localTrailPheromone;
        int routeLength;
        double heuristicsWeight;
        double qProbability;

        static Random random = new Random();


        //constructor: initializes the problem parameters
        public Problem(int[][] points, int[] startingPoint, int colonySize = 30, int nnCount = 5,
            int iterations = 50, double heuristicsWeight = 2, double qProbability = 0.9, double lTrailPheromone = 0.1, int[] endPoint = null)
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


            //sets the number of iterations
            this.iterations = iterations;

            //Converts the points passed as arrays into nodes
            this.nodes = points.Select(x => new Node(x)).ToDictionary(x => x.NodeId, x => x);
            this.routeLength = this.nodes.Count;

            //sets colony size
            this.colonySize = colonySize;

            //sets the nearest neighbours list count
            this.nnCount = nnCount < this.routeLength  ? nnCount : this.routeLength - 1;

            //sets the heuristicsWeight
            this.heuristicsWeight = heuristicsWeight;

            this.SetStartingNode(startingPoint);

            if (endPoint != null)
            {
                this.SetEndNode(endPoint);
            }

            //builds arcs for the graph and the nearest neighbours list
            this.BuildGraph();

            //sets local pheromone update "rate"
            this.localTrailPheromone = lTrailPheromone;

            //initializes pheromone ammount in all arcs
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

        public void SetEndNode(int[] endPoint)
        {
            if (endPoint != null)
            {
                if (this.nodes.Count() < 1)
                {
                    //TODO devolver error de lista de nodos vacia
                }

                this.endNodeId = nodes.Where(x => x.Value.Lat == endPoint[0] && x.Value.Lng == endPoint[1]).FirstOrDefault().Value.NodeId;

                if (this.endNodeId < 1)
                {
                    //TODO devolver error que el nodo elegido como inicial no existe
                }
            }
        }

        private void BuildGraph()
        {
            foreach (var initNode in nodes)
            {
                var nearestNodes = new List<KeyValuePair<int, double>>();

                foreach (var endNode in nodes.Where(currNode => currNode.Value.NodeId != initNode.Value.NodeId ))
                {
                    //builds the arcs for the graph
                    var arcNodes = new Node[] { initNode.Value, endNode.Value };
                    //var arcNodesIds = new int[2] { initNode.Key, endNode.Key };
                    var arc = new Arc(arcNodes, this.heuristicsWeight);
                    this.arcsInfo.Add(arc);

                    //adds the node and distance to search for the nearest neighbours of initNode
                    nearestNodes.Add(new KeyValuePair<int, double>(endNode.Key, arc.Distance));
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
            var nnDistance = (double)0;

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

            this.arcsInfo.ForEach(x => { x.Pheromone = this.initialPheromone; x.InitialPheromone = this.initialPheromone; });
        }

        //starts the search
        public int[][] FindRoute()
        {
            for (var i = 0; i < this.iterations; i++)
            {
                var bestAnt = this.ContructSolutions();
                bestAnt = this.LocalSearch(bestAnt);

                var iterationBest = new BestSoFar()
                {
                    Route = bestAnt.Route,
                    RouteLength = bestAnt.RouteLength,
                    Iteration = i
                };

                if (this.bestSoFar == null)
                {
                    this.bestSoFar = iterationBest;
                }
                else
                {
                    this.bestSoFar = this.bestSoFar.RouteLength < iterationBest.RouteLength ? this.bestSoFar : iterationBest;
                }
                //this.UpdateStats(i);
                //TODO this.UpdatePheromone();
            }

            var bestRoute = this.bestSoFar.GetRoute();
            return bestRoute;
        }

        private Ant ContructSolutions()
        {
            //create ant colony
            List<Ant> antColony = Enumerable.Range(0, this.colonySize).Select(x => new Ant(this.nodes, this.localTrailPheromone, this.startingNode.NodeId, this.endNodeId)).ToList();

            //find routes for all ants in the iteration
            for (var i = 0; i < this.nodes.Count; i++)
            {
                foreach (var ant in antColony)
                {
                    ant.FindNextNode(this.arcsInfo, this.nearestNodes, this.qProbability);
                }
            }

            foreach(var ant in antColony)
            {
                ant.AddNodeToRoute(ant.Route[0], this.arcsInfo.Find(x => x.InitNodeId == ant.Route[ant.Route.Count - 1] && x.EndNodeId == ant.Route[0]).Distance);
                
                //since the normal algorithm does not have an ending point and I need one I have to transform the ones the ants found into valid routes
                if (this.endNodeId != null)
                {
                    ant.BuildValidRoute(this.arcsInfo);
                }
            }                     

            //get best route in the iteration
            var bestAnt = antColony.OrderBy(ant => ant.RouteLength).FirstOrDefault();
            
            return bestAnt;
        }

        private Ant LocalSearch(Ant bestAnt)
        {
            var localSearchBest = bestAnt;
            return localSearchBest;
        }

        private void UpdatePheromone()
        {
            foreach(var node in this.bestSoFar.Route)
            {
                
            }
        }

        #region Useful but probably unused        

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
        #endregion
    }

    internal class BestSoFar
    {
        public List<int> Route { get; set; }
        public double RouteLength { get; set; }
        public int Iteration { get; set; }

        public int[][] GetRoute()
        {

            //TODO
            return new int[1][] { new int[2] { 2, 4 } };
        }
    }
}
