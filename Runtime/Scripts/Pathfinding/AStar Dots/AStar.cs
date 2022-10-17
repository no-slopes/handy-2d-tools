
using UnityEngine;
using UnityEngine.Events;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using System.Collections.Generic;
using static H2DT.Utils.Math;
using H2DT.Debugging;

namespace H2DT.Pathfinding.AStar.Dots
{
    public class AStar
    {
        int2 _gridSize;

        // protected Grid<PathNode> _grid;
        protected NativeArray<DotsGridCell> _cells;

        public NativeArray<DotsGridCell> AstarGrid => _cells;

        protected Vector2 _origin;
        protected Dictionary<PathfindingDirections, int2> _directionOffsets = new Dictionary<PathfindingDirections, int2>();

        public bool debug;

        public AStar(int width, int height, Vector2 origin)
        {
            _origin = origin;
            _gridSize = new int2(width, height);
            _cells = new NativeArray<DotsGridCell>(_gridSize.x * _gridSize.y, Allocator.Persistent);

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    DotsGridCell gridCell = new DotsGridCell();
                    gridCell.x = x;
                    gridCell.y = y;

                    gridCell.blockage = false;

                    gridCell.index = CalculateIndex(x, y, _gridSize.x);

                    _cells[gridCell.index] = gridCell;
                }
            }

            _directionOffsets[PathfindingDirections.North] = new int2(0, +1);
            _directionOffsets[PathfindingDirections.NorthEast] = new int2(+1, +1);
            _directionOffsets[PathfindingDirections.East] = new int2(+1, 0);
            _directionOffsets[PathfindingDirections.SouthEast] = new int2(+1, -1);
            _directionOffsets[PathfindingDirections.South] = new int2(0, -1);
            _directionOffsets[PathfindingDirections.SouthWest] = new int2(-1, -1);
            _directionOffsets[PathfindingDirections.West] = new int2(-1, 0);
            _directionOffsets[PathfindingDirections.NorthWest] = new int2(-1, +1);
        }

        public void Dismiss()
        {
            _cells.Dispose();
        }

        public void SetBlockage(Vector2Int worldPos, bool isBlockage)
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i].index == CalculateIndex(worldPos.x, worldPos.y, _gridSize.x))
                {

                    DotsGridCell gridCell = new DotsGridCell();
                    gridCell.x = worldPos.x;
                    gridCell.y = worldPos.y;

                    gridCell.blockage = isBlockage;

                    gridCell.index = CalculateIndex(worldPos.x, worldPos.y, _gridSize.x);

                    _cells[gridCell.index] = gridCell;
                    break;
                }
            }
        }

        public DotsGridCell GetGridCell(Vector2Int worldPos)
        {
            return GetGridCell(worldPos.x, worldPos.y);
        }

        public DotsGridCell GetGridCell(int x, int y)
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                if (_cells[i].index == CalculateIndex(x, y, _gridSize.x))
                {
                    return _cells[i];
                }
            }

            return new DotsGridCell();
        }

        public void ResetBlockages()
        {
            for (int i = 0; i < _cells.Length; i++)
            {
                DotsGridCell gridCell = _cells[i];
                gridCell.blockage = false;
                _cells[gridCell.index] = gridCell;
            }
        }

        protected int CalculateIndex(int x, int y, int gridWidth)
        {
            return x + y * gridWidth;
        }

        public void FindPath(int2 startPosition, int2 endPosition, UnityAction<List<Vector2>> OnResult, List<PathfindingDirections> directions = null)
        {
            float startTime = Time.realtimeSinceStartup;

            NativeList<int2> result = new NativeList<int2>(Allocator.TempJob);

            NativeArray<DotsGridCell> grid = new NativeArray<DotsGridCell>(_gridSize.x * _gridSize.y, Allocator.TempJob);
            NativeArray<int2> neighbourOffsetArray = NeighbourOffsetsFromDirections(directions, Allocator.TempJob);

            grid.CopyFrom(_cells);

            FindPathJob findPathJob = new FindPathJob
            {
                startPosition = startPosition,
                endPosition = endPosition,
                gridSize = this._gridSize,
                grid = grid,
                neighbourOffsetArray = neighbourOffsetArray,
                result = result,
            };

            JobHandle handle = findPathJob.Schedule();

            handle.Complete();

            OnResult.Invoke(ConvertIntoWorldPath(result));

            DebugPath(startPosition, endPosition, startTime);

            result.Dispose();
            grid.Dispose();
            neighbourOffsetArray.Dispose();
        }

        protected NativeArray<int2> NeighbourOffsetsFromDirections(List<PathfindingDirections> directions, Allocator allocator)
        {
            if (directions == null || directions.Count == 0)
            {
                return FullNeighoursOffset(allocator);
            }
            else
            {
                NativeArray<int2> neighboursOffset = new NativeArray<int2>(directions.Count, Allocator.TempJob);

                for (int i = 0; i < directions.Count; i++)
                {
                    neighboursOffset[i] = _directionOffsets[directions[i]];
                }
                return neighboursOffset;
            }
        }

        protected NativeArray<int2> FullNeighoursOffset(Allocator allocator)
        {
            return new NativeArray<int2>(new int2[]{
                new int2(-1,0), // Left
                new int2(+1,0), // Right
                new int2(0,+1), // Up
                new int2(0,-1), // Down
                new int2(-1,-1), // Left Down
                new int2(-1,+1), // Left Up
                new int2(+1, -1), // Right Down
                new int2(+1, +1), // Right Up
            }, allocator);
        }

        protected List<Vector2> ConvertIntoWorldPath(NativeList<int2> nativePath)
        {
            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < nativePath.Length; i++)
            {
                Vector2 pos = new Vector2(nativePath[i].x, nativePath[i].y);
                Vector2 centeredPos = CenterInWorldPosition(WorldIntoCellPosAddingOrigin(pos, _origin));
                path.Add(centeredPos);
            }

            return path;
        }

        protected void DebugPath(int2 startingPos, int2 endPos, float startTime)
        {
            if (!debug) return;

            float elapsedTime = ((Time.realtimeSinceStartup - startTime) * 1000f);
            Log.Success($"Executed find path from {startingPos} to {endPos}");
            Log.Warning($"Elapsed time: {elapsedTime}");
        }
    }
}