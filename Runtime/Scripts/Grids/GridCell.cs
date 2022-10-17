using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.Grids
{
    public class GridCell<T>
    {
        #region Fields

        protected Grid<T> _grid;
        protected int _x;
        protected int _y;
        protected Vector2 _rawGridPosition;
        protected T _cellValue;

        #endregion

        #region  Properties

        public T cellValue { get { return _cellValue; } set { _cellValue = value; } }

        #endregion

        #region Getters

        public int x => _x;
        public int y => _y;
        public Vector2 rawGridPosition => _rawGridPosition;

        #endregion

        #region Contructors

        public GridCell(Grid<T> grid, int x, int y)
        {
            _grid = grid;
            _x = x;
            _y = y;
            _rawGridPosition = new Vector2(x, y);
        }

        #endregion

        #region Logic

        public Vector2 GetWorldPosition()
        {
            return _grid.GetWorldPosition(_x, _y);
        }

        public Vector2 GetCenteredWorldPosition()
        {
            return _grid.GetWorldPositionCentered(_x, _y);
        }

        #endregion
    }
}