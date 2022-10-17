using System.Collections;
using System.Collections.Generic;
using H2DT.Enums;
using UnityEngine;

namespace H2DT.Spawning
{
    [System.Serializable]
    public class SpawnInfo
    {
        [SerializeField]
        protected Transform _point;

        [SerializeField]
        protected Direction _facingDirection;

        public Transform point => _point;
        public Direction facingDirection => _facingDirection;

    }
}
