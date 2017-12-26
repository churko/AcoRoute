using System;
using System.Collections.Generic;

namespace Engine
{
    public class Ant
    {
        public int RouteLength { get; set; }

        private List<Node> route;
        public void AddNodeToRoute (Node node)
        {
            route.Add(node);
        }

        public List<Node> ShowRoute ()
        {
            return this.route;
        }

        public List<Node> Visited { get; set; }
       
    }
}
