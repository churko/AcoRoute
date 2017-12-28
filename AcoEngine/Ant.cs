using System;
using System.Collections.Generic;
using System.Linq;

namespace AcoEngine
{
    public class Ant
    {
        static int aId;

        private int id;
        public int Id => id;

        private List<Node> nodes;

        private int routeLength;
        public int RouteLength => routeLength; 

        private List<Node> route;
        private void AddNodeToRoute(Node node, int distance)
        {
            this.id = aId;
            this.routeLength += distance;
            this.route.Add(node);
            this.nodes.Remove(node);
            aId++;
        }

        public List<Node> ShowRoute()
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

        public void FindNextNode(Dictionary<Node[], Arc> arcsInfo, Dictionary<Node, List<Node>> nearestNodes)
        {
            var lastNode = this.route.Last();
            var lastNodeNN = nearestNodes[lastNode];
            var validNN = lastNodeNN.Where(node => !this.route.Any(rNode => rNode.Lat == node.Lat && rNode.Lng == node.Lng)).ToList();
            if (validNN.Count() > 0)
            {
                var nnKeys = validNN.Select(x => new Node[] { lastNode, x }).ToList();
                var nnArcs = nnKeys.Where(k => arcsInfo.ContainsKey(k)).ToDictionary(k=> k[1],x=>arcsInfo[x]);
                
            }
        }

    }
}
