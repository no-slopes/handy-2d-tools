using System.Collections;
using System.Collections.Generic;
using H2DT.Debugging;
using UnityEngine;
using UnityEngine.Events;
using static H2DT.Utils.Math;

namespace H2DT.Grids
{
    public class HandyGrid<T>
    {
        #region Fields

        protected Vector2 _origin;

        protected int _width;
        protected int _height;

        protected int _cellSize;

        protected int[,] _grid;
        protected Dictionary<Vector2Int, HandyGridCell<T>> _cells = new Dictionary<Vector2Int, HandyGridCell<T>>();

        #endregion

        #region  Properties

        #endregion

        #region  Getters

        public int width => _width;
        public int height => _height;

        public int cellSize => _cellSize;

        #endregion

        #region  Constructors

        public HandyGrid(Vector2 origin, int width, int height, int cellSize) : this(width, height, cellSize)
        {
            _origin = origin;
        }

        public HandyGrid(int width, int height, int cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
        }

        #endregion

        #region Logic

        public void Initialize(UnityAction<HandyGridCell<T>> OnCellCreated = null)
        {
            _grid = new int[_width, _height];

            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                for (int y = 0; y < _grid.GetLength(1); y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    HandyGridCell<T> cell = new HandyGridCell<T>(this, x, y);
                    _cells.Add(pos, cell);
                    OnCellCreated?.Invoke(cell);
                }
            }
        }

        public HandyGridCell<T> GetCell(Vector2 worldPos)
        {
            Vector2Int pos = WorldIntoCellPosDeducingOrigin(worldPos, _origin);

            if (!ValidatePosInsideGrid(pos)) return null;

            return _cells[pos];
        }

        public HandyGridCell<T> GetCell(int x, int y)
        {
            Vector2Int pos = new Vector2Int(x, y);

            if (!ValidatePosInsideGrid(pos)) return null;

            return _cells[pos];
        }

        public void TryGetCell(Vector2 worldPos, out HandyGridCell<T> cell)
        {
            Vector2Int pos = WorldIntoCellPosDeducingOrigin(worldPos, _origin);
            _cells.TryGetValue(pos, out cell);
        }

        public Vector2 GetWorldPosition(int x, int y)
        {
            return new Vector2(x, y) * _cellSize + _origin;
        }

        public Vector2 GetWorldPositionCentered(int x, int y)
        {
            return GetWorldPosition(x, y) + new Vector2(_cellSize, _cellSize) * .5f;
        }

        protected bool ValidatePosInsideGrid(Vector2Int pos)
        {
            if (pos.x < 0) return false;
            if (pos.x > _width) return false;
            if (pos.y < 0) return false;
            if (pos.y > _height) return false;

            return true;
        }

        #endregion

    }
}
