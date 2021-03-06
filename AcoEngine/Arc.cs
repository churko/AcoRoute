﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcoEngine
{
    public class Arc
    {
        readonly double distance;
        public double Distance => distance;

        double pheromone = 0;

        double choiceInfo;
        public double ChoiceInfo => choiceInfo;

        double heuristicWeightInP;

        public int InitNodeId { get; }
        public int EndNodeId { get; }
        public double InitialPheromone { get; set; }

        public Arc(Node[] arcNodes, double heuristicWeight)
        {
            this.InitNodeId = arcNodes[0].NodeId;
            this.EndNodeId = arcNodes[1].NodeId;
            //calculates manhattan distance between the nodes
            var latDist = Math.Abs(arcNodes[0].Lat - arcNodes[1].Lat);
            var lngDist = Math.Abs(arcNodes[0].Lng - arcNodes[1].Lng);
            this.distance = lngDist + latDist;
            this.heuristicWeightInP = Math.Pow(((double)1 / this.distance), heuristicWeight);
        }        

        public double Pheromone {
            get
            {
                return this.pheromone;
            }
            set
            {
                this.pheromone = value;
                this.UpdateChoiceInfo();
            }
        }        

        public void UpdateChoiceInfo()
        {
            this.choiceInfo = this.pheromone * this.heuristicWeightInP;
        }
    }
}
