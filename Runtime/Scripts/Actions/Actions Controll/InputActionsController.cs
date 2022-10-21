using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using H2DT.NaughtyAttributes;

namespace H2DT.Actions
{
    /// <summary>
    /// Represents any entity wich is an ActionsController and
    /// receives player Input.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public abstract class InputActionsController<T> : ActionsController<T>
    {
        #region Inspector

        [Header("Input Action")]
        [Tooltip("Uncheck this so that input won't be processed. You also can use EnableInput(), DisableInput() and ToggleInput() methods to enable/disable input.")]
        [SerializeField]
        [Space]
        private bool _listenInput = true;

        [Space]
        [SerializeField]
        protected PlayerInput _playerInput;

        #endregion

        #region Fields  

        private string _previousMapName;
        private string _currentMapName;

        #endregion

        #region Properties

        protected bool listenInput { get => _listenInput; set => _listenInput = value; }

        protected string previousMapName => _previousMapName;
        protected string currentMapName => _currentMapName;

        protected override abstract bool canEmit { get; }

        #endregion

        #region Getters

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (_playerInput == null) FindComponent<PlayerInput>(ref _playerInput);
        }

        #endregion

        #region Map management

        public virtual void ChangeMap(string mapName)
        {
            _previousMapName = _currentMapName;
            _currentMapName = mapName;
            _playerInput.SwitchCurrentActionMap(_currentMapName);
        }

        #endregion
    }
}
