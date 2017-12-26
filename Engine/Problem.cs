using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
    public class Problem
    {
        List<Node> nodes;
        Dictionary<Node[], Arc> arcInfo;
        int nnCount;
        Dictionary<Node, List<Node>> nearestNodes;
        Node startingNode;
        Node endNode;
        int colonySize;
        int iterations; //termination condition
        BestSoFar bestSoFar;
        double initialPheromone;
        int routeLength;
        double heuristicsWeigth;
        double qProbability;

        

        //constructor: initializes the problem parameters
        public Problem(int[][] points, int[] startingPoint, int colonySize = 30, int nnCount = 5,
            int iterations = 50, double heuristicsWeight = 2, double qProbability = 0.1 , int[] endPoint = null)
        {
            //initializes the number of iterations
            this.iterations = iterations;

            //Converts the points passed as arrays into nodes
            //foreach (double[] point in points)
            //{
            //    nodes.Add(new Node(point));
            //}

            //Converts the points passed as arrays into nodes
            this.nodes = Enumerable.Range(0, points.Length).Select(x => new Node(points[x])).ToList();
            this.routeLength = this.nodes.Count;

            //initializes colony size
            this.colonySize = colonySize;           

            //initializes the nearest neighbours list count
            this.nnCount = nnCount;

            //initializes the heuristicsWeight
            this.heuristicsWeigth = heuristicsWeight;

            this.startingNode = new Node(startingPoint);

            //builds arcs for the graph and the nearest neighbours list
            this.BuildGraph();

            //initializes the pheromones
            this.InitializePheromone();
        }       

        private void BuildGraph ()
        {
            foreach (var initNode in nodes)
            {
                var nearestNodes = new List<KeyValuePair<Node, int>>();

                foreach (var endNode in nodes.Where(n => !n.Equals(initNode)))
                {
                    //builds the arcs for the graph
                    var arcPoints = new Node[] { initNode, endNode };
                    var arc = new Arc(arcPoints, this.initialPheromone, this.qProbability);
                    arcInfo.Add(arcPoints, arc);

                    //adds the node and distance to search for the nearest neighbours of initNode
                    nearestNodes.Add(new KeyValuePair<Node, int>(endNode, arc.Distance));
                }

                //get the (nnCount)nth nodes with the shortest distance to initNodes
                nearestNodes.Sort((x, y) => x.Value.CompareTo(y.Value));
                nearestNodes.Take(this.nnCount);

                //builds the nearestNodes dictionary
                //var nearestNeighbours = new List<Node>();
                //foreach (var node in nearestNodes)
                //{
                //    nearestNeighbours.Add(node.Key);
                //}
                //this.nearestNodes.Add(initNode, nearestNeighbours);

                //builds the nearestNodes dictionary
                var nearestNeighbours = new List<Node>();
                nearestNeighbours = Enumerable.Range(0, this.nnCount).Select(x => nearestNodes[x].Key).ToList();
                this.nearestNodes.Add(initNode, nearestNeighbours);
            }
        }

        private void InitializePheromone()
        {
            var nodeAux = new Node[this.routeLength];
            this.nodes.CopyTo(nodeAux);
            var initialNodes = nodeAux.ToList();

            var tour = new List<Node>();
            var nnDistance = 0;

            var nextNode = this.startingNode;
            tour.Add(nextNode);
            for (var i = 1; i < this.routeLength; i++)
            {
                var pairs = this.nodes.Where(node => !tour.Contains(node)).Select(node => new Node[] { nextNode, node }).ToList(); //TODO check if this where works
                var nodeArcs = this.arcInfo.Where(x => pairs.Contains(x.Key)).ToDictionary(x => x.Key[1], x => x.Value.Distance).ToList();
                nodeArcs.Sort((x, y) => x.Value.CompareTo(y.Value));
                nextNode = nodeArcs[0].Key;
                nnDistance += nodeArcs[0].Value;
                tour.Add(nextNode);
            }

            nnDistance += this.arcInfo[new Node[] { nextNode, this.startingNode }].Distance;

            this.initialPheromone = 1 / (this.routeLength * nnDistance);

        }

        //starts the search
        public double[][] FindRoute()
        {
            for (var i = 0; i < this.iterations; i++)
            {
                this.bestSoFar = this.ContructSolutions();
                this.bestSoFar = this.TwoOpt();
                this.UpdateStats(i);
                this.UpdatePheromone();
            }

            double[][] finalRoute = null;
            return finalRoute;
        }

        private BestSoFar ContructSolutions()
        {
            List<Ant> antColony = Enumerable.Range(0,this.colonySize).Select(x => new Ant(this.nodes)).ToList();

            for (var i = 0; i < this.nodes.Count; i++)
            {
                foreach (var ant in antColony)
                {
                    ant.FindNextNode(this.arcInfo, this.nearestNodes);
                    //TODO termine aca
                }
            }
            
            var iterationBest = new BestSoFar();
            return iterationBest.Distance < this.bestSoFar.Distance ? iterationBest : this.bestSoFar;
        }

        private BestSoFar TwoOpt()
        {
            var localBest = new BestSoFar();
            return localBest.Distance < this.bestSoFar.Distance ? localBest : this.bestSoFar ;
        }

        private void UpdateStats(int iteration)
        {
            this.bestSoFar.Iteration = iteration;
        }

        private void UpdatePheromone()
        {
            throw new NotImplementedException();
        }
    }

    internal class BestSoFar
    {
        public List<Node> Route { get; set; }
        public int Distance { get; set; }
        public int Iteration { get; set; }

    }
}
