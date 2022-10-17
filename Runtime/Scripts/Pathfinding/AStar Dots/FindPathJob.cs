using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

namespace H2DT.Pathfinding.AStar.Dots
{
    [BurstCompile]
    public struct FindPathJob : IJob
    {
        #region Constants

        const int MOVE_STRAIGHT_COST = 10;
        const int MOVE_DIAGONAL_COST = 14;

        #endregion

        public int2 startPosition;
        public int2 endPosition;

        public int2 gridSize;
        public NativeArray<DotsGridCell> grid;
        public NativeArray<int2> neighbourOffsetArray;

        public NativeList<int2> result;

        public void Execute()
        {
            int startNodeIndex = CalculateIndex(startPosition.x, startPosition.y, gridSize.x);
            int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);

            NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.index = CalculateIndex(x, y, gridSize.x);
                    pathNode.gridCell = grid[pathNode.index];
                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = CalculateDistanceCost(new int2(x, y), gridSize.x);
                    pathNode.CalculateFCost();
                    pathNode.cameFromIndex = -1;

                    pathNodeArray[pathNode.index] = pathNode;
                }
            }

            PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodeArray[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodeArray);
                PathNode currentNode = pathNodeArray[currentNodeIndex];

                if (currentNodeIndex == endNodeIndex)
                {
                    // reached destination. Hurray!
                    break;
                }

                // Remove current node from open list
                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsetArray.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsetArray[i];
                    int2 neighbourPosition = new int2(currentNode.gridCell.x + neighbourOffset.x, currentNode.gridCell.y + neighbourOffset.y);

                    if (!IsPositionInsideGrid(neighbourPosition, gridSize))
                    {
                        continue;
                    }

                    int neighbourIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                    if (closedList.Contains(neighbourIndex))
                    {
                        // Already searched this node
                        continue;
                    }

                    PathNode neighbourNode = pathNodeArray[neighbourIndex];

                    if (neighbourNode.gridCell.blockage)
                    {
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.gridCell.x, currentNode.gridCell.y);

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);

                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                        pathNodeArray[neighbourIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNode.index))
                        {
                            openList.Add(neighbourNode.index);
                        }
                    }
                }
            }

            PathNode endNode = pathNodeArray[endNodeIndex];

            if (endNode.cameFromIndex == -1)
            {
                // No path found.
            }
            else
            {
                CalculatePath(pathNodeArray, endNode);
            }

            pathNodeArray.Dispose();
            openList.Dispose();
            closedList.Dispose();
        }

        public void CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
        {
            if (endNode.cameFromIndex == -1) return;

            result.Add(new int2(endNode.gridCell.x, endNode.gridCell.y));

            PathNode currentNode = endNode;

            while (currentNode.cameFromIndex != -1)
            {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromIndex];
                result.Add(new int2(cameFromNode.gridCell.x, cameFromNode.gridCell.y));
                currentNode = cameFromNode;
            }

            int start = 0;
            int end = result.Length - 1;

            while (start < end)
            {
                // swap arr[start] and arr[end]
                int2 temp = result[start];
                result[start] = result[end];
                result[end] = temp;
                start = start + 1;
                end = end - 1;
            }
        }

        public int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        public int CalculateDistanceCost(int2 aPosition, int2 bPosition)
        {
            int xDistance = math.abs(aPosition.x - bPosition.x);
            int yDistance = math.abs(aPosition.y - bPosition.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        public int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray)
        {
            PathNode lowestCostPathNode = pathNodeArray[openList[0]];

            for (int i = 0; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodeArray[openList[i]];
                if (testPathNode.fCost < lowestCostPathNode.fCost)
                {
                    lowestCostPathNode = testPathNode;
                }
            }

            return lowestCostPathNode.index;
        }

        public bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize)
        {
            return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x &&
            gridPosition.y < gridSize.y;
        }
    }
}