using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace H2DT.Combat.Aims
{
    public abstract class Aim : HandyComponent
    {

        #region Inspector

        [Header("Aim needed Objects")]
        [Space]
        [SerializeField]
        private Transform _shootPoint;

        [Header("Configuration")]
        [Space]
        [SerializeField]
        protected bool _readFromMouse = true;

        #endregion       

        #region Fields

        private Vector2 _aimDirection;
        private Vector2 _currentMousePos;
        private Vector2 _lastMousePos;

        #endregion

        #region Properties

        public Vector2 aimDirection { get { return _aimDirection; } set { _aimDirection = value; } }

        protected Vector2 currentMousePos => _currentMousePos;
        protected bool readFromMouse => _readFromMouse;

        #endregion

        #region Getters

        public Transform shootPoint => _shootPoint;

        #endregion

        #region Mono         

        protected void FixedUpdate()
        {
            ReadMousePosition();
        }

        #endregion

        #region Logic

        protected void ReadMousePosition()
        {
            if (!_readFromMouse) return;

            _currentMousePos = Mouse.current.position.ReadValue();

            if (_lastMousePos != _currentMousePos)
            {
                _lastMousePos = _currentMousePos;
                _aimDirection = (Camera.main.ScreenToWorldPoint(_currentMousePos) - transform.position);
            }
        }

        #endregion
    }
}
