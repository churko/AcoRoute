using System;
using System.Collections.Generic;
using System.Linq;

namespace AcoEngine
{
    public class Ant
    {
        private List<Node> nodes;

        private int routeLength;
        public int RouteLength { get => routeLength; }

        private List<Node> route;
        private void AddNodeToRoute (Node node, int distance)
        {
            this.routeLength += distance;
            this.route.Add(node);
            this.nodes.Remove(node);
        }

        public List<Node> ShowRoute ()
        {
            return this.route;
        }

        public Ant(List<Node> nodes)
        {
            this.routeLength = 0;
            this.nodes = nodes;
            this.SetFirstNode(this.nodes);
        }

        private void SetFirstNode(List<Node> nodes)
        {
            Random rnd = new Random();
            var firstNode = nodes[rnd.Next(nodes.Count - 1)];
            this.AddNodeToRoute(firstNode, 0);
        }

        public void FindNextNode(Dictionary<Node[], Arc> arcInfo, Dictionary<Node, List<Node>> nearestNodes)
        {
            var lastNode = this.route.Last();
        }

    }
}
