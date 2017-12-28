using System.Threading;

namespace AcoEngine
{
    public struct Node
    {

        static int nextId;
        public int NodeId { get; private set; }

        readonly int lat;

        readonly int lng;

        public Node(int[] coord)
        {
            this.NodeId = Interlocked.Increment(ref nextId);
            this.lat = coord[0];
            this.lng = coord[1];
        }

        public int Lat => lat;

        public int Lng => lng;
    }
}
