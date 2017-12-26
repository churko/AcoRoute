using System;

namespace Engine
{
    public class Node
    {
        readonly int lat;

        readonly int lng;

        public Node(decimal[] coord)
        {
            //transforms the latitude and longitude into integer numbers
            var correction = Convert.ToInt32(Math.Pow(10, 7));
            this.lat = Convert.ToInt32(coord[0] * correction);
            this.lng = Convert.ToInt32(coord[1] * correction);
        }

        public int Lat => lat;

        public int Lng => lng;
    }
}