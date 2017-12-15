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

        public Problem(decimal[][] points, decimal[] startingPoint, int colonySize = 30, int nearNodes = 5, decimal initPheromone = 1, decimal[] endPoint = null)
        {
            foreach (decimal[] point in points)
            {
                nodes.Add(new Node(point));
            }

            foreach (var initNode in nodes)
            {
                var nearestNodes = new List<KeyValuePair<Node, int>>();

                foreach (var endNode in nodes.Where(n => n != initNode))
                {
                    var arcPoints = new Node[] { initNode, endNode };
                    var arc = new Arc(arcPoints, initPheromone);
                    

                    arcInfo.Add(arcPoints , arc);
                    nearestNodes.Add(new KeyValuePair<Node, int>(endNode, arc.Distance));
                }

                nearestNodes.Sort((x, y) => x.Value.CompareTo(y.Value));
                nearestNodes.Take(nearNodes);

                var nearestNeighbours = new List<Node>();
                foreach (var node in nearestNodes)
                {
                    nearestNeighbours.Add(node.Key);
                }

                this.nearestNodes.Add(initNode, nearestNeighbours);
            }

        }

        public List<Node> FindRoute()
        {



            List<Node> tour = new List<Node>();
            return tour;
        }
    }
}
