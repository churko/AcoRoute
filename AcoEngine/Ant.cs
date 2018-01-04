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

        static Random random = new Random();

        List<int> route = new List<int>();
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
            //var minKey = nodes.Min(kvp => kvp.Key);
            //var rnd = random.Next(minKey, minKey + nodes.Count - 1);
            var rnd = random.Next(1, nodes.Count);
            var firstNode = nodes[rnd];
            this.AddNodeToRoute(firstNode, 0);
        }

        public void FindNextNode(List<Arc> arcsInfo, Dictionary<int, List<int>> nearestNodes, double qProbability)
        {
            var nextNodeId = new int();
            var lastNode = this.route.Last();
            var qRandom = random.NextDouble();


            if (qRandom < qProbability) //if q probability search the best possible
            {
                var bestArc = arcsInfo.Where(arc => arc.InitNodeId == lastNode && !this.route.Any(rNodeId => rNodeId == arc.EndNodeId)).OrderByDescending(x => x.ChoiceInfo).FirstOrDefault();
                nextNodeId = bestArc.EndNodeId;
            }
            else
            {
                //nearest neighbour search
                var lastNodeNN = nearestNodes[lastNode];
                var validNN = lastNodeNN.Where(nodeId => !this.route.Any(rNodeId => rNodeId == nodeId)).ToList();
                if (validNN.Count() > 0)
                {
                    var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode && validNN.Any(x => x == arc.EndNodeId)).OrderBy(x => x.ChoiceInfo).ToList();
                    nextNodeId = this.GetNextNode(validArcs);

                   
                }
                else //search in all if not nearest neighbour available
                {
                    var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode && !this.route.Any(rNodeId => rNodeId == arc.EndNodeId)).OrderBy(x => x.ChoiceInfo).ToList();
                    nextNodeId = this.GetNextNode(validArcs);
                }
            }
            this.route.Add(nextNodeId);
        }

        private int GetNextNode(List<Arc> validArcs)
        {
            var totalChoiceInfo = validArcs.Sum(x => x.ChoiceInfo);
            int? nextNodeId = null;
            double selectionProbability = (double)0;
            double rnd = GetRandomNumber(0, totalChoiceInfo);
            foreach (var arc in validArcs)
            {
                if (selectionProbability < rnd)
                {
                    nextNodeId = arc.EndNodeId;
                    selectionProbability += arc.ChoiceInfo;
                }
                else
                {
                    break;
                }
            }
            return (int)nextNodeId;
        }

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return (random.NextDouble() * (maximum - minimum)) + minimum;
        }

    }
}
