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
                foreach (var endNode in nodes.Where(n => n != initNode))
                {
                    Node[] arcPoints = new Node[] { initNode, endNode };
                    arcInfo.Add(arcPoints , new Arc(arcPoints, initPheromone));
                }
            }

        }

        public List<Node> FindRoute()
        {



            List<Node> tour = new List<Node>();
            return tour;
        }
    }
}
