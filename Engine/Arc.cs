using System;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public class Arc
    {
        readonly int distance;

        decimal pheromone;

        decimal choiceInfo;

        public Arc (Node[] arcNodes, decimal initPheromone)
        {
            var latDist = Math.Abs(arcNodes[0].Lat - arcNodes[1].Lat);
            var lngDist = Math.Abs(arcNodes[0].Lng - arcNodes[1].Lng);
            this.distance = lngDist + latDist;
            this.pheromone = initPheromone;
            this.choiceInfo = 0;
        }

        public int Distance => distance;

        public decimal Pheromone { get => pheromone; set => pheromone = value; }
        public decimal ChoiceInfo { get => choiceInfo; set => choiceInfo = value; }

        
    }
}
