using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Actions.FSM
{
    public abstract class MonolessStateMachine<T0>
    {
        #region Fields

        // Actor
        private T0 _actor;

        // States
        private MonolessState<T0> _currentState;
        private MonolessState<T0> _defaultState;
        private MonolessState<T0> _previousState;
        private List<MonolessState<T0>> _states = new List<MonolessState<T0>>();

        private string _currentStateName;

        // Events
        public UnityEvent<MonolessState<T0>> StateChanged = new UnityEvent<MonolessState<T0>>();

        #endregion

        #region Properties

        protected List<MonolessState<T0>> states => _states;

        #endregion   

        #region Getters

        public T0 actor => _actor;

        public MonolessState<T0> currentState => _currentState;
        public MonolessState<T0> defaultState => _defaultState;
        public MonolessState<T0> previousState => _previousState;

        public string currentStateName => _currentStateName;

        #endregion

        #region Constructors

        public void SetUp(T0 actor)
        {
            _actor = actor;

            RecognizeStates();
            LoadStates();
        }

        #endregion

        #region Loading

        protected virtual void LoadStates()
        {

            foreach (MonolessState<T0> state in _states)
            {
                state.InternalLoad(this);
                state.LoadAction?.Invoke();
            }

        }

        #endregion

        #region Machine's Logic

        /// <summary>
        /// Defines a given state as active
        /// </summary>
        /// <param name="state"> The state to be set as active </param>
        /// <param name="forceInterruption"> If an uninterruptible state should be interrupted </param>
        public virtual void RequestStateChange(MonolessState<T0> state, bool forceInterruption = false)
        {
            if (_currentState != null && !_currentState.interruptible && !forceInterruption) return; // State cannot be interrupted, but will if forced.

            ChangeState(state);
        }

        /// <summary>
        /// Ends a state transitioning it into the target state.
        /// Case no target is provided, trasitions into the default state.
        /// </summary>
        /// <param name="target"> The state to be set as active </param>
        public virtual void EndState(MonolessState<T0> target = null)
        {
            if (target != null)
            {
                ChangeState(target);
            }
            else
            {
                ChangeState(_defaultState);
            }

        }

        protected virtual void ChangeState(MonolessState<T0> state)
        {

            if (state == _currentState || state == null) return; // Should not change 

            _previousState = _currentState; // Defines the previous state

            _currentState?.OnExitAction?.Invoke(); // Exiting current state

            _currentState = state; // Changing current state

            _currentState.OnEnterAction?.Invoke(); // Initializing new state

            _currentStateName = currentState.name;

            StateChanged.Invoke(currentState);
        }

        #endregion

        #region Abstractions

        protected abstract void RecognizeStates();

        #endregion

    }
}
