using System.Threading;

namespace AcoEngine
{
    public struct Node
    {

        static int nextId;
        public int NodeId { get; private set; }

        readonly double lat;

        readonly double lng;

        public Node(double[] coord)
        {
            this.NodeId = Interlocked.Increment(ref nextId);
            this.lat = coord[0];
            this.lng = coord[1];
        }

        public double Lat => lat;

        public double Lng => lng;
    }
}
