using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace H2DT.Management.Levels
{
    [System.Serializable]
    public class LevelCamera
    {
        [SerializeField]
        protected CinemachineVirtualCamera _camera;

        [SerializeField]
        protected PolygonCollider2D _confinerCollider;

        public CinemachineVirtualCamera camera => _camera;
        public PolygonCollider2D confinerCollider => _confinerCollider;
    }
}
