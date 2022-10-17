using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using H2DT.Generics.Transitions;
using UnityEngine.Events;
using System.Threading.Tasks;
using H2DT.Management.Booting;

namespace H2DT.Management.Gameplay
{
    /// <summary>
    /// This abstract component exists to perform transitions between gameplay states.
    /// </summary>
    public abstract class GameplayTransitionSubject : BooterSubject, ITransitionPerformer
    {
        #region Inspector

        [Header("Gameplay")]
        [Space]
        [SerializeField]
        private GameplayHandler _handler;

        [SerializeField]
        private GameplayStateType _gameplayStateType;

        #endregion

        #region Properties

        protected GameplayHandler gameplayHandler => _handler;
        protected GameplayStateType gameplayStateType => _gameplayStateType;

        #endregion

        #region  Mono

        protected virtual void OnEnable()
        {
            _handler.RegisterTransitionSubject(_gameplayStateType, this);
        }

        protected virtual void OnDisable()
        {
            _handler.UnregisterTransitionSubject(_gameplayStateType, this); // unregisters it self as a subject
        }

        #endregion

        #region ITransitionPerformer

        /// <summary>
        /// A task to perform the entrance transition. As this is abstract
        /// it awaits for the child execution.
        /// </summary>
        /// <returns></returns>
        public async Task PlayEnterTransition()
        {
            await OnPlayEnterTransition();
        }

        #endregion

        /// <summary>
        /// A task to perform the exit transition. As this is abstract
        /// it awaits for the child execution.
        /// </summary>
        /// <returns></returns>
        public async Task PlayExitTransition()
        {
            await OnPlayExitTransition();
        }

        #region Abstractions

        /// <summary>
        /// A Callback for when the PlayEnterTransition is called.
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnPlayEnterTransition();

        /// <summary>
        /// A Callback for when the PlayExitTransition is called.
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnPlayExitTransition();

        #endregion
    }
}
