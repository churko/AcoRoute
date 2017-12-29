﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcoEngine
{
    public class Arc
    {
        readonly int distance;

        double pheromone;

        double choiceInfo;

        double qProbability;

        public int InitNodeId { get; }
        public int EndNodeId { get; }

        public Arc(Node[] arcNodes, double initPheromone, double qProbability)
        {
            this.InitNodeId = arcNodes[0].NodeId;
            this.EndNodeId = arcNodes[1].NodeId;
            //calculates manhattan distance between the nodes
            var latDist = Math.Abs(arcNodes[0].Lat - arcNodes[1].Lat);
            var lngDist = Math.Abs(arcNodes[0].Lng - arcNodes[1].Lng);
            this.distance = lngDist + latDist;

            this.pheromone = initPheromone;

            this.choiceInfo = this.pheromone * (1 / distance);

            this.qProbability = qProbability;
        }

        public int Distance => distance;

        public double Pheromone { get => pheromone; set => pheromone = value; }
        public double ChoiceInfo { get => choiceInfo; }

        public void UpdateChoiceInfo()
        {
            this.choiceInfo = this.pheromone * Math.Pow((1 / this.distance), this.qProbability);
        }


    }
}
