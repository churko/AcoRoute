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

        Dictionary<int,Node> nodes;

        double pheromoneEvaporation;

        double routeDistance;
        public double RouteDistance => routeDistance;

        static Random random = new Random();

        List<int> route = new List<int>();
        public List<int> Route => route;

        int problemInitNodeId;
        int? problemEndNodeId;

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return (random.NextDouble() * (maximum - minimum)) + minimum;
        }


        public Ant(Dictionary<int, Node> nodes, double pheromoneEvaporation, int pInitNodeId, int? pEndNodeId )
        {
            this.AntId = Interlocked.Increment(ref nextId);
            this.routeDistance = 0;
            this.pheromoneEvaporation = pheromoneEvaporation;
            this.problemInitNodeId = pInitNodeId;
            this.problemEndNodeId = pEndNodeId;
            this.nodes = nodes;
            this.SetFirstNode(this.nodes);
        }

        public void AddNodeToRoute(int nodeId, double distance)
        {
            this.routeDistance += distance;
            this.route.Add(nodeId);
            //this.nodes.Remove(nodeId);
        } 

        private void SetFirstNode(Dictionary<int,Node> nodes)
        {
            //var minKey = nodes.Min(kvp => kvp.Key);
            //var rnd = random.Next(minKey, minKey + nodes.Count - 1);
            var rnd = random.Next(1, nodes.Count);
            var firstNode = nodes[rnd];
            this.AddNodeToRoute(firstNode.NodeId, 0);
        }

        public void FindNextNode(List<Arc> arcsInfo, Dictionary<int, List<int>> nearestNodes, double qProbability)
        {
            Arc nextNodeArc = null;
            var lastNode = this.route.Last();
            var qRandom = random.NextDouble();

            if (qRandom < qProbability) //if q probability search the best possible
            {
                nextNodeArc = arcsInfo.Where(arc => arc.InitNodeId == lastNode && !this.route.Any(rNodeId => rNodeId == arc.EndNodeId)).OrderByDescending(x => x.ChoiceInfo).FirstOrDefault();                
                //nextNodeArc = arcsInfo.Where(arc => arc.InitNodeId == lastNode).OrderByDescending(x => x.ChoiceInfo).FirstOrDefault();
            }
            else
            {
                //nearest neighbour search
                var lastNodeNN = nearestNodes[lastNode];
                var validNN = lastNodeNN.Where(nodeId => !this.route.Any(rNodeId => rNodeId == nodeId)).ToList();
                //var validNN = nearestNodes[lastNode];

                if (validNN.Count() > 0)
                {
                    var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode && validNN.Any(x => x == arc.EndNodeId)).OrderBy(x => x.ChoiceInfo).ToList();
                    nextNodeArc = this.GetNextNode(validArcs);                                  
                }
                else //search in all if not nearest neighbour available
                {
                    var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode && !this.route.Any(rNodeId => rNodeId == arc.EndNodeId)).OrderBy(x => x.ChoiceInfo).ToList();
                    //var validArcs = arcsInfo.Where(arc => arc.InitNodeId == lastNode).OrderBy(x => x.ChoiceInfo).ToList();

                    nextNodeArc = this.GetNextNode(validArcs);                    
                }
            }
            this.AddNodeToRoute(nextNodeArc.EndNodeId, nextNodeArc.Distance);
            this.LocalPheromoneUpdate(nextNodeArc);
        }

        private void LocalPheromoneUpdate(Arc arcToUpdate)
        {
            arcToUpdate.Pheromone = (1 - this.pheromoneEvaporation) * arcToUpdate.Pheromone + arcToUpdate.InitialPheromone;
        }

        private Arc GetNextNode(List<Arc> validArcs)
        {
            var totalChoiceInfo = validArcs.Sum(x => x.ChoiceInfo);
            Arc nextNodeArc = null;
            double selectionProbability = (double)0;
            double rnd = GetRandomNumber(0, totalChoiceInfo);
            foreach (var arc in validArcs)
            {
                if (selectionProbability < rnd)
                {
                    nextNodeArc = arc;
                    selectionProbability += arc.ChoiceInfo;
                }
                else
                {
                    break;
                }
            }
            return nextNodeArc;
        }

        public void BuildValidRoute(List<Arc> arcsInfo)
        {
            var pEndIndex = this.route.FindIndex(x => x == this.problemEndNodeId);
            var pInitialIndex = this.route.FindLastIndex(x => x == this.problemInitNodeId);
            double deltaDistance = 0;

            if(pEndIndex != pInitialIndex - 1)
            {
                var leftEndId = pEndIndex == 0 ? this.route[this.route.Count - 2] : this.route[pEndIndex - 1];
                var rightEndId = this.route[pEndIndex + 1];
                deltaDistance -= arcsInfo.Find(x => x.InitNodeId == leftEndId && x.EndNodeId == this.problemEndNodeId).Distance;
                deltaDistance -= arcsInfo.Find(x => x.InitNodeId == this.problemEndNodeId && x.EndNodeId == rightEndId).Distance;

                var leftInitId = this.route[pInitialIndex - 1];
                deltaDistance -= arcsInfo.Find(x => x.InitNodeId == leftInitId && x.EndNodeId == this.problemInitNodeId).Distance;

                deltaDistance += arcsInfo.Find(x => x.InitNodeId == leftEndId && x.EndNodeId == rightEndId).Distance;
                deltaDistance += arcsInfo.Find(x => x.InitNodeId == leftInitId && x.EndNodeId == this.problemEndNodeId).Distance;
                
                //since I have an end node I do not want to go back to the starting node so this distance does not matter to me hence I don't add it
                //deltaDistance += arcsInfo.Find(x => x.InitNodeId == this.problemEndNodeId && x.EndNodeId == this.problemInitNodeId).Distance;

                this.route.RemoveAt(pEndIndex);
                this.route.Insert(pInitialIndex, (int)this.problemEndNodeId);

                if(pEndIndex == 0)
                {
                    this.route.RemoveAt(this.route.Count - 1);
                    this.route.Add(this.route[0]);
                }                
            }
            else
            {
                //since I have an end node I do not want to go back to the starting node so this distance does not matter to me hence I substract it
                deltaDistance -= arcsInfo.Find(x => x.InitNodeId == this.problemEndNodeId && x.EndNodeId == this.problemInitNodeId).Distance;
            }

            this.routeDistance += deltaDistance;
            //TODO i don't remember if this already takes away the repeating node, will have to check
        }
    }
}
