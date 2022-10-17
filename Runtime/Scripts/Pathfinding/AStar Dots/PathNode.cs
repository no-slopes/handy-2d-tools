namespace H2DT.Pathfinding.AStar.Dots
{
    public struct PathNode
    {
        public DotsGridCell gridCell;

        public int gCost;
        public int hCost;
        public int fCost;

        public int index;
        public int cameFromIndex;

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
    }
}