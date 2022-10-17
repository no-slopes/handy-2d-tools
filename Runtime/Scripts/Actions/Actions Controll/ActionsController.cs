using System.Collections;
using System.Collections.Generic;
using H2DT.Actions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Actions
{
    /// <summary>
    /// Represents any entity wich can emit actions for whoever wants to listen.
    /// </summary>
    public abstract class ActionsController<T> : HandyComponent
    {
        #region Inspector
        [Header("Actions in General")]
        [Tooltip("Unmark this if you want to completely disable actions emission. You can also use EnableEmission(), DisableEmission() and ToggleEmission() methods to enable/disable emission.")]
        [SerializeField]
        [Space]
        protected bool _canEmitActions = true;

        #endregion

        #region Getters 

        /// <summary>
        /// A lambda check wich determines if actions can be
        /// emitted.
        /// </summary>
        /// <value> true if can emit actions </value>
        protected abstract bool canEmit { get; }

        #endregion

        #region Callbacks 

        /// <summary>
        /// Enables emission of actions. 
        /// </summary>
        public void EnableEmission()
        {
            _canEmitActions = true;
        }

        /// <summary>
        /// Disables emission of actions.
        /// </summary>
        public void DisableEmission()
        {
            _canEmitActions = false;
        }

        /// <summary>
        /// Toggles emission of actions.
        /// </summary>
        public void ToggleEmission()
        {
            _canEmitActions = !_canEmitActions;
        }

        #endregion

        #region Action Emission

        /// <summary>
        /// Emits an event
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        protected virtual void EmitAction(UnityEvent unityEvent)
        {
            if (!canEmit) return;

            unityEvent.Invoke();
        }

        /// <summary>
        /// Emits a event passing a Vector2 value.
        /// </summary>
        /// <param name="unityEvent"> The Event </param>
        /// <param name="vector">The Vector2 value</param>
        protected virtual void EmitAction(UnityEvent<Vector2> unityEvent, Vector2 vector)
        {
            if (!canEmit) return;

            unityEvent.Invoke(vector);
        }

        /// <summary>
        /// Emits a event passing a float value.
        /// </summary>
        /// <param name="unityEvent"> The Event </param>
        /// <param name="number">The float value</param>
        protected virtual void EmitAction(UnityEvent<float> unityEvent, float number)
        {
            if (!canEmit) return;

            unityEvent.Invoke(number);
        }

        /// <summary>
        /// Emits a event passing a int value.
        /// </summary>
        /// <param name="unityEvent"> The Event </param>
        /// <param name="number">The int value</param>
        protected virtual void EmitAction(UnityEvent<int> unityEvent, int number)
        {
            if (!canEmit) return;

            unityEvent.Invoke(number);
        }

        /// <summary>
        /// Emits a event passing a string value.
        /// </summary>
        /// <param name="unityEvent"> The Event </param>
        /// <param name="text">The string value</param>
        protected virtual void EmitAction(UnityEvent<string> unityEvent, string text)
        {
            if (!canEmit) return;

            unityEvent.Invoke(text);
        }

        /// <summary>
        /// Emits a event passing an GoodBatchGames.Handy2dTools.Actions.Actor.
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        /// <param name="actor"> The Actor </param>
        protected virtual void EmitAction(UnityEvent<T> unityEvent, T actor)
        {
            if (!canEmit) return;

            unityEvent.Invoke(actor);
        }

        /// <summary>
        /// Emits a event passing an GoodBatchGames.Handy2dTools.Actions.Actor and a Vector2 value.
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        /// <param name="actor"> The Actor </param>
        /// <param name="vector"> The Vector2 </param>
        protected virtual void EmitAction(UnityEvent<T, Vector2> unityEvent, T actor, Vector2 vector)
        {
            if (!canEmit) return;

            unityEvent.Invoke(actor, vector);
        }

        /// <summary>
        /// Emits a event passing an GoodBatchGames.Handy2dTools.Actions.Actor and a float value.
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        /// <param name="actor"> The Actor </param>
        /// <param name="number"> The float </param>
        protected virtual void EmitAction(UnityEvent<T, float> unityEvent, T actor, float number)
        {
            if (!canEmit) return;

            unityEvent.Invoke(actor, number);
        }

        /// <summary>
        /// Emits a event passing an GoodBatchGames.Handy2dTools.Actions.Actor and a int value.
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        /// <param name="actor"> The Actor </param>
        /// <param name="number"> The int </param>
        protected virtual void EmitAction(UnityEvent<T, int> unityEvent, T actor, int number)
        {
            if (!canEmit) return;

            unityEvent.Invoke(actor, number);
        }

        /// <summary>
        /// Emits a event passing an GoodBatchGames.Handy2dTools.Actions.Actor and a string value.
        /// </summary>
        /// <param name="unityEvent"> The event </param>
        /// <param name="actor"> The Actor </param>
        /// <param name="text"> The string </param>
        protected virtual void EmitAction(UnityEvent<T, string> unityEvent, T actor, string text)
        {
            if (!canEmit) return;

            unityEvent.Invoke(actor, text);
        }

        #endregion
    }
}
