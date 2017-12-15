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

        Dictionary<Node, List<Node>> nearestNodes;

        List<Ant> antColony;

        Node startingNode;

        Node endNode;

        int colonySize;

        int iterations;

        Dictionary<List<Node>, int> bestSoFar;

        //constructor: initializes the problem parameters
        public Problem(decimal[][] points, decimal[] startingPoint, int colonySize = 30, int nearNodes = 5, decimal initPheromone = 1, 
            int iterations = 50, decimal[] endPoint = null)
        {
            //Converts the points passed as arrays into nodes
            foreach (decimal[] point in points)
            {
                nodes.Add(new Node(point));
            }

            foreach (var initNode in nodes)
            {
                var nearestNodes = new List<KeyValuePair<Node, int>>();

                foreach (var endNode in nodes.Where(n => n != initNode))
                {
                    //builds the arcs for the graph
                    var arcPoints = new Node[] { initNode, endNode };
                    var arc = new Arc(arcPoints, initPheromone);          
                    arcInfo.Add(arcPoints , arc);

                    //adds the node and distance to search for the nearest neighbours of initNode
                    nearestNodes.Add(new KeyValuePair<Node, int>(endNode, arc.Distance));
                }

                //get the (nearNodes)nth nodes with the shortest distance to initNodes
                nearestNodes.Sort((x, y) => x.Value.CompareTo(y.Value));
                nearestNodes.Take(nearNodes);

                //builds the nearestNodes dictionary
                var nearestNeighbours = new List<Node>();
                foreach (var node in nearestNodes)
                {
                    nearestNeighbours.Add(node.Key);
                }
                this.nearestNodes.Add(initNode, nearestNeighbours);
            }

            //initializes the number of iterations
            this.iterations = iterations;
        }

        //starts the search
        public List<Node> FindRoute()
        {
            List<Node> tour = new List<Node>();
            return tour;
        }
    }
}
