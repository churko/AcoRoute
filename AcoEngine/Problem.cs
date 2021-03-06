﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcoEngine
{
    public class Problem
    {
        List<Node> nodes;
        List<Arc> arcsInfo = new List<Arc>();
        int nnCount;
        Dictionary<int, List<int>> nearestNodes = new Dictionary<int, List<int>>();
        Node startingNode;
        int? endNodeId;
        int colonySize;
        int iterations; //termination condition
        BestSoFar bestSoFar;
        double initialPheromone;
        double pheromoneEvaporation;
        int routeDistance;
        double heuristicsWeight;
        double qProbability;

        static Random random = new Random();


        //constructor: initializes the problem parameters
        public Problem(double[][] points, double[] startingPoint, double[] endPoint, int colonySize = 10, int nnCount = 5,
            int iterations = 50, double heuristicsWeight = 2, double qProbability = 0.9, double pheromoneEvaporation = 0.1)
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

            //sets the probability
            this.qProbability = qProbability;

            //sets the number of iterations
            this.iterations = iterations;

            //Converts the points passed as arrays into nodes
            this.nodes = points.Select(x => new Node(x)).ToList();
            this.routeDistance = this.nodes.Count;

            //sets colony size
            this.colonySize = colonySize;

            //sets the nearest neighbours list count
            this.nnCount = nnCount < this.routeDistance  ? nnCount : this.routeDistance - 1;

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
            this.pheromoneEvaporation = pheromoneEvaporation;

        }

        public void SetStartingNode(double[] startingPoint)
        {
            if (this.nodes.Count() < 1)
            {
                //TODO devolver error de lista de nodos vacia
            }

            this.startingNode = nodes.Where(x => x.Lat == startingPoint[0] && x.Lng == startingPoint[1]).FirstOrDefault();

            if (this.startingNode.NodeId < 1)
            {
                //TODO devolver error que el nodo elegido como inicial no existe
            }
        }

        public void SetEndNode(double[] endPoint)
        {
            if (endPoint != null)
            {
                if (this.nodes.Count() < 1)
                {
                    //TODO devolver error de lista de nodos vacia
                }

                this.endNodeId = nodes.Where(x => x.Lat == endPoint[0] && x.Lng == endPoint[1]).FirstOrDefault().NodeId;

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

                foreach (var endNode in nodes.Where(currNode => currNode.NodeId != initNode.NodeId ))
                {
                    //builds the arcs for the graph
                    var arcNodes = new Node[] { initNode, endNode };
                    //var arcNodesIds = new int[2] { initNode.Key, endNode.Key };
                    var arc = new Arc(arcNodes, this.heuristicsWeight);
                    this.arcsInfo.Add(arc);

                    //adds the node and distance to search for the nearest neighbours of initNode
                    nearestNodes.Add(new KeyValuePair<int, double>(endNode.NodeId, arc.Distance));
                }

                //get the (nnCount)nth nodes with the shortest distance to initNodes
                nearestNodes.Sort((x, y) => x.Value.CompareTo(y.Value));
                nearestNodes.Take(this.nnCount);

                //builds the nearestNodes dictionary
                var nearestNeighbours = new List<int>();
                nearestNeighbours = Enumerable.Range(0, this.nnCount).Select(x => nearestNodes[x].Key).ToList();
                this.nearestNodes.Add(initNode.NodeId, nearestNeighbours);
            }
        }

        private void InitializePheromone()
        {
            var initialNodes = this.nodes.ToArray();

            var tour = new List<Node>();
            var nnDistance = (double)0;

            var nextNode = this.startingNode; 
            tour.Add(nextNode);
            for (var i = 1; i < this.routeDistance; i++)
            {
                var pairs = this.nodes.Where(currNode => !tour.Any(tNode => tNode.NodeId == currNode.NodeId)).Select(currNode => new Node[] { nextNode, currNode }).ToList();
                var nodeArcs = this.arcsInfo.Where(x => pairs.Any(pNode => pNode[0].NodeId == x.InitNodeId && pNode[1].NodeId == x.EndNodeId)).OrderBy(o => o.Distance).ToList();
                nextNode = this.nodes.Find(x => x.NodeId == nodeArcs[0].EndNodeId);
                nnDistance += nodeArcs[0].Distance;
                tour.Add(nextNode);
            }
            nnDistance += this.arcsInfo.Where(x => x.InitNodeId == nextNode.NodeId && x.EndNodeId == this.startingNode.NodeId).FirstOrDefault().Distance;

            this.initialPheromone = Convert.ToDouble(1) / Convert.ToDouble(this.routeDistance * nnDistance);

            this.arcsInfo.ForEach(x => { x.Pheromone = this.initialPheromone; x.InitialPheromone = this.initialPheromone; });
        }

        //starts the search
        public double[][] FindRoute()
        {
            
            for (var i = 0; i < this.iterations; i++)
            {
                //initializes pheromone ammount in all arcs
                this.InitializePheromone();

                var bestAnt = this.ConstructSolutions();
                var iterationBest = this.LocalSearch(bestAnt);
                iterationBest.Iteration = i;

                if (this.bestSoFar == null || this.bestSoFar.Route.Count != this.nodes.Count)
                {
                    this.bestSoFar = iterationBest;
                }
                else
                {
                    this.bestSoFar = this.bestSoFar.RouteDistance < iterationBest.RouteDistance ? this.bestSoFar : iterationBest;
                }
                //this.UpdateStats(i);
                this.UpdatePheromone();
            }

            var bestRoute = this.bestSoFar.GetOrderedRoute();
            var bestRouteArray = this.RouteToArray(bestRoute);

            this.bestSoFar = null; //if I run the method w/o reinstancing the Problem object best so far must be reinitialized or it will carry to the next run

            return bestRouteArray;
        }

        private double[][] RouteToArray(List<int> route)
        {
            var latLngList = new List<double[]>();
            foreach (var nodeId in route)
            {
                var node = this.nodes.Find(x => x.NodeId == nodeId);
                var latLng = new double[3] { node.Lat, node.Lng, node.PersonId };
                latLngList.Add(latLng);
            }

            var finalArray = latLngList.ToArray();
            return finalArray;
        }

        private Ant ConstructSolutions()
        {
            //create ant colony
            List<Ant> antColony = Enumerable.Range(0, this.colonySize).Select(x => new Ant(this.nodes, this.arcsInfo, this.pheromoneEvaporation, this.startingNode.NodeId, this.endNodeId)).ToList();

            //find routes for all ants in the iteration
            for (var i = 0; i < this.nodes.Count - 1; i++)
            {
                foreach (var ant in antColony)
                {
                    ant.FindNextNode(this.nearestNodes, this.qProbability);
                }
            }

            foreach (var ant in antColony)
            {
                ant.AddNodeToRoute(ant.Route[0], this.arcsInfo.Find(x => x.InitNodeId == ant.Route[ant.Route.Count - 1] && x.EndNodeId == ant.Route[0]).Distance);
                
                //since the normal algorithm does not have an ending point and I need one I have to transform the ones the ants found into valid routes
                if (this.endNodeId != null)
                {
                    ant.BuildValidRoute(this.arcsInfo);
                }
            }                     

            //get best route in the iteration
            var bestAnt = antColony.OrderBy(ant => ant.RouteDistance).FirstOrDefault();         

            return bestAnt;
        }

        private BestSoFar LocalSearch(Ant bestAnt)
        {
            var iterationBest = new BestSoFar(this.startingNode.NodeId, this.endNodeId)
            {
                Route = bestAnt.Route,
                RouteDistance = bestAnt.RouteDistance
            };

            //if (iterationBest.Route.Count > 4)
            //{
            //    for (var i = 0; i < this.iterations; i++)
            //    {
            //        var children = FindChildren(iterationBest);
            //        var bestChild = children.OrderBy(x => x.RouteDistance).FirstOrDefault();
            //        if (bestChild.RouteDistance > iterationBest.RouteDistance)
            //        {
            //            break;
            //        }

            //        iterationBest = bestChild;
            //    }
            //}

            return iterationBest;
        }

        private List<BestSoFar> FindChildren(BestSoFar bestRoute)
        {
            var children = new List<BestSoFar>();
            int ceiling = this.endNodeId == null ? (bestRoute.Route.Count - 2) / 2 : (bestRoute.Route.Count - 3) / 2;
            for (var i = 0; i < ceiling; i ++)
            {
                var index = 1 + i * 2;
                var route = bestRoute.Route.ToList();
                var distance = bestRoute.RouteDistance;

                distance -= this.arcsInfo.Find(x => x.InitNodeId == route[index - 1] && x.EndNodeId == route[index]).Distance;
                distance -= this.arcsInfo.Find(x => x.InitNodeId == route[index + 1] && x.EndNodeId == route[index + 2]).Distance;

                var tmp = route[index];
                route[index] = route[index+1];
                route[index+1] = tmp;

                distance += this.arcsInfo.Find(x => x.InitNodeId == route[index - 1] && x.EndNodeId == route[index]).Distance;
                distance += this.arcsInfo.Find(x => x.InitNodeId == route[index + 1] && x.EndNodeId == route[index + 2]).Distance;

                var child = new BestSoFar(this.startingNode.NodeId, this.endNodeId)
                {
                    Route = route,
                    RouteDistance = distance,
                    LocalSearch = true
                };

                children.Add(child);
            }
            
            return children;
        }


        private void UpdatePheromone()
        {
            var deltaEvaporation = this.pheromoneEvaporation / this.bestSoFar.RouteDistance;
            var route = this.bestSoFar.Route;
            for (var i = 0; i< this.bestSoFar.Route.Count - 2; i++)
            {
                var bsfArc = arcsInfo.Find(x => x.InitNodeId == route[i] && x.EndNodeId == route[i + 1]);
                bsfArc.Pheromone = (1 - this.pheromoneEvaporation) * bsfArc.Pheromone + deltaEvaporation;
            }
        }

        #region Useful but probably unused        

        private bool ValidatePoints(double[] point)
        {
            if(point.GetLength(0) != 2)
            {
                return false;
            }
            return true;
        }

        private bool ValidatePoints(double[][] points)
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
        List<int> route = new List<int>();
        public List<int> Route { get => route; set => route = value; }
        public double RouteDistance { get; set; }
        public int Iteration { get => iteration; set => iteration = value; }
        public bool LocalSearch { get; set; }

        int iteration;
        int startingNodeId;
        int? endNodeId;

        public BestSoFar(int startingNodeId, int? endNodeId)
        {
            this.startingNodeId = startingNodeId;
            this.endNodeId = endNodeId;
            this.LocalSearch = false;
        }
        

        public List<int> GetOrderedRoute()
        {
            var routeCount = this.route.Count;
            this.route.RemoveAt(routeCount - 1);
            routeCount--;            
            var initialIndex = this.route.FindIndex(x => x == this.startingNodeId);            
            var subroute = this.route.GetRange(initialIndex, routeCount - initialIndex);
            subroute.AddRange(this.route.GetRange(0, initialIndex));
            if(this.endNodeId == null)
            {
                subroute.Add(subroute[0]);
                routeCount++;
            }
            
            return subroute;
        }
    }
}
