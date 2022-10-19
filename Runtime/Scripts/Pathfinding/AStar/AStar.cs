using System.Collections;
using System.Collections.Generic;
using H2DT.Debugging;
using H2DT.Grids;
using UnityEngine;
using static H2DT.Utils.Math;

namespace H2DT.Pathfinding.AStar
{
    public class AStar
    {
        #region Constants

        protected const int MOVE_STRAIGHT_COST = 10;
        protected const int MOVE_DIAGONAL_COST = 14;

        #endregion

        #region  Fields

        protected HandyGrid<PathNode> _grid;
        protected Vector2 _origin;

        protected List<PathNode> _openList;
        protected List<PathNode> _closedList;

        protected Dictionary<PathfindingDirections, Vector2> _directionOffsets = new Dictionary<PathfindingDirections, Vector2>();

        #endregion

        #region  Properties

        public bool debug { get; set; }

        #endregion


        #region Getters

        public HandyGrid<PathNode> grid => _grid;

        #endregion

        #region Constructors

        public AStar(int width, int height, Vector2 origin)
        {
            _grid = new HandyGrid<PathNode>(origin, width, height, 1);
            _origin = origin;
            _grid.Initialize(OnGridCellCreated);

            _directionOffsets.Add(PathfindingDirections.North, new Vector2(0, 1));
            _directionOffsets.Add(PathfindingDirections.NorthEast, new Vector2(1, 1));
            _directionOffsets.Add(PathfindingDirections.East, new Vector2(1, 0));
            _directionOffsets.Add(PathfindingDirections.SouthEast, new Vector2(1, -1));
            _directionOffsets.Add(PathfindingDirections.South, new Vector2(0, -1));
            _directionOffsets.Add(PathfindingDirections.SouthWest, new Vector2(-1, -1));
            _directionOffsets.Add(PathfindingDirections.West, new Vector2(-1, 0));
            _directionOffsets.Add(PathfindingDirections.NorthWest, new Vector2(-1, -1));
        }

        #endregion

        #region  Contructor Callbacks

        protected void OnGridCellCreated(HandyGridCell<PathNode> cell)
        {
            cell.cellValue = new PathNode(cell);
        }

        #endregion

        #region Logic

        public PathNode GetPathNode(Vector2 worldPos)
        {
            return _grid.GetCell(worldPos)?.cellValue;
        }

        public PathNode GetPathNode(int x, int y)
        {
            return _grid.GetCell(x, y)?.cellValue;
        }

        public List<Vector2> FindPath(Vector2 startPos, Vector2 endPos, List<PathfindingDirections> allowedDirections)
        {
            float startTime = Time.realtimeSinceStartup;

            PathNode startNode = _grid.GetCell(startPos).cellValue;
            PathNode endNode = _grid.GetCell(endPos).cellValue;

            _openList = new List<PathNode> { startNode };
            _closedList = new List<PathNode>();

            for (int x = 0; x < _grid.width; x++)
            {
                for (int y = 0; y < _grid.height; y++)
                {
                    PathNode node = _grid.GetCell(x, y).cellValue;
                    node.gCost = int.MaxValue;
                    node.cameFromNode = null;
                }
            }

            startNode.gCost = 0;
            startNode.hCost = CalculateDistanceCost(startNode, endNode);

            // All this to scan the grid

            while (_openList.Count > 0)
            {
                PathNode currentNode = LowestFCostNode(_openList);

                if (currentNode == endNode)
                {
                    // Reached final node
                    break;
                }

                _openList.Remove(currentNode);
                _closedList.Add(currentNode);

                foreach (PathNode neighbourNode in GetNeighbours(currentNode, allowedDirections))
                {
                    if (_closedList.Contains(neighbourNode)) continue; // We have added this neighbour already
                    if (!neighbourNode.walkable) { _closedList.Add(neighbourNode); continue; }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);

                        if (!_openList.Contains(neighbourNode))
                        {
                            _openList.Add(neighbourNode);
                        }
                    }
                }
            }

            List<Vector2> path = GeneratePath(endNode);

            DebugPath(startPos, endPos, startTime);

            return path;
        }

        protected List<PathNode> GetNeighbours(PathNode node, List<PathfindingDirections> allowedDirections)
        {
            List<PathNode> neighbours = new List<PathNode>();


            foreach (PathfindingDirections direction in allowedDirections)
            {
                Vector2 neighbourPos = node.gridCell.GetWorldPosition() + _directionOffsets[direction];
                PathNode neighbourNode = GetPathNode(neighbourPos);

                if (neighbourNode == null) continue;

                neighbours.Add(neighbourNode);
            }

            return neighbours;
        }

        protected List<Vector2> GeneratePath(PathNode endNode)
        {
            List<Vector2> path = new List<Vector2>();

            if (endNode.cameFromNode == null)
            {
                return path;
            }

            path.Add(endNode.gridCell.GetCenteredWorldPosition());

            PathNode currentNode = endNode;

            while (currentNode.cameFromNode != null)
            {
                path.Add(currentNode.cameFromNode.gridCell.GetCenteredWorldPosition());
                currentNode = currentNode.cameFromNode;
            }

            path.Reverse();

            return path;
        }

        protected int CalculateDistanceCost(PathNode a, PathNode b)
        {
            int xDistance = Mathf.Abs(a.gridCell.x - b.gridCell.x);
            int yDistance = Mathf.Abs(a.gridCell.y - b.gridCell.y);

            int remaining = Mathf.Abs(xDistance - yDistance);

            return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
        }

        protected PathNode LowestFCostNode(List<PathNode> nodes)
        {
            PathNode lowest = nodes[0];

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].fCost < lowest.fCost)
                {
                    lowest = nodes[i];
                }
            }

            return lowest;
        }

        #endregion        

        protected void DebugPath(Vector2 startingPos, Vector2 endPos, float startTime)
        {
            if (!debug) return;

            float elapsedTime = ((Time.realtimeSinceStartup - startTime) * 1000f);
            Log.Success($"Executed find path from {startingPos} to {endPos}");
            Log.Warning($"Elapsed time: {elapsedTime}");
        }
    }
}
