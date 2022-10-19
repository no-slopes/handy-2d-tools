using System.Collections;
using System.Collections.Generic;
using H2DT.Grids;
using UnityEngine;

namespace H2DT.Pathfinding.AStar
{
    public class PathNode
    {

        protected HandyGridCell<PathNode> _gridCell;

        protected int _gCost;
        protected int _hCost;

        protected PathNode _cameFromNode;

        protected bool _walkable;

        public HandyGridCell<PathNode> gridCell => _gridCell;

        public int gCost { get { return _gCost; } set { _gCost = value; } }
        public int hCost { get { return _hCost; } set { _hCost = value; } }

        public int fCost => _gCost + _hCost;

        public bool walkable { get { return _walkable; } set { _walkable = value; } }

        public PathNode cameFromNode { get { return _cameFromNode; } set { _cameFromNode = value; } }

        public PathNode(HandyGridCell<PathNode> gridCell)
        {
            _gridCell = gridCell;
            _walkable = true;
        }

        public override string ToString()
        {
            return _gridCell.x + " - " + _gridCell.y;
        }
    }
}