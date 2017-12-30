using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AcoEngine
{
    public class Ant
    {
        static int nextId;

        public int AntId { get; private set; }

        private Dictionary<int,Node> nodes;

        private int routeLength;
        public int RouteLength => routeLength; 

        private List<int> route;
        private void AddNodeToRoute(Node node, int distance)
        {
            this.AntId = Interlocked.Increment(ref nextId);
            this.routeLength += distance;
            this.route.Add(node.NodeId);
            this.nodes.Remove(node.NodeId);
        }

        private void AddNodeToRoute(int nodeId, int distance)
        {
            this.AntId = Interlocked.Increment(ref nextId);
            this.routeLength += distance;
            this.route.Add(nodeId);
            this.nodes.Remove(nodeId);
        }

        public List<int> ShowRoute()
        {
            return this.route;
        }

        public Ant(Dictionary<int,Node> nodes)
        {
            this.routeLength = 0;
            this.nodes = nodes;
            this.SetFirstNode(this.nodes);
        }

        private void SetFirstNode(Dictionary<int,Node> nodes)
        {
            Random rnd = new Random();
            var firstNode = nodes[rnd.Next(nodes.Count - 1)];
            this.AddNodeToRoute(firstNode, 0);
        }

        public void FindNextNode(List<Arc> arcsInfo, Dictionary<int, List<Node>> nearestNodes)
        {
            int nextNodeId;
            var lastNode = this.route.Last();
            var lastNodeNN = nearestNodes[lastNode];
            var validNN = lastNodeNN.Where(node => !this.route.Any(rNodeId => rNodeId == node.NodeId)).Select(x=> x.NodeId).ToList();
            if (validNN.Count() > 0)
            {
                var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode && validNN.Any(x => x == arc.EndNodeId )).OrderBy(x => x.ChoiceInfo).ToList();
                var totalChoiceInfo = validArcs.Sum(x => x.ChoiceInfo);                
                double selectionProbability = 0;
                double rnd = Problem.GetRandomNumber(0, totalChoiceInfo);
                foreach(var arc in validArcs)
                {
                    if (selectionProbability < rnd)
                    {
                        nextNodeId = arc.EndNodeId;
                        selectionProbability += selectionProbability; 
                    }
                    else
                    {
                        break;
                    }
                }


                //var arcProb = arcsInfo.Where(arc => validNN.Any(nNodeId => arc.EndNodeId == nNodeId)).OrderBy(x=>x.ChoiceInfo).ToDictionary(x => x.EndNodeId, x => x.ChoiceInfo);

            }
            else
            {
                //todo que hacer si no hay mas disponibilidad de nearest neighbour
            }
        }

    }
}
