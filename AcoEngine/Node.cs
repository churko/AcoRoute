namespace AcoEngine
{
    public struct Node
    {
        readonly int lat;

        readonly int lng;

        public Node(int[] coord)
        {
            this.lat = coord[0];
            this.lng = coord[1];
        }

        public int Lat => lat;

        public int Lng => lng;
    }
}
