using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using H2DT.NaughtyAttributes;
using H2DT.Debugging;

namespace H2DT.Actions.FSM
{
    /// <summary>
    /// The state machine
    /// </summary>
    [AddComponentMenu("Handy 2D Tools/FSM/State Machine")]
    [DefaultExecutionOrder(100)]
    public class StateMachine<T0> : HandyComponent
    {
        #region Fields

        /// <summary>
        /// The state machine's actor instance. This is set in the Machine's SetUp method.
        /// </summary>
        [Label("Actor")]
        [Tooltip("The game object wich holds an Actor component")]
        [SerializeField]
        [ReadOnly]
        [Space]
        protected T0 _actor;

        /// <summary>
        /// The current machine's status of the MachineStatus enum type. 
        /// </summary>
        [Header("Status")]
        [Label("Current status")]
        [Tooltip("The current machine's status. Should be On, Off, Paused, Loading or Ready.")]
        [SerializeField]
        [ReadOnly]
        protected MachineStatus _status = MachineStatus.Off;

        /// <summary>
        /// The current state name
        /// </summary>
        [Label("Current State")]
        [Tooltip("The machine's current state name")]
        [ReadOnly]
        [SerializeField]
        [ShowIf("showCurrentState")]
        protected string _currentStateName;

        [Header("States")]
        [Required("You must define a Default State")]
        [Tooltip("The machine's default state")]
        [SerializeField]
        protected State<T0> _defaultState;

        /// <summary>s
        /// Visual feedback about states attached at this game object
        /// </summary>
        [Label("Recognized States")]
        [Tooltip("This do NOT represent the states list that will be really used by the machine and is only a visual feedback for you. The machine will handle states recognition from inside its life cycle.")]
        [ReadOnly]
        [SerializeField]
        [Space]
        protected List<State<T0>> _recognizedStates = new List<State<T0>>();

        [Foldout("Available Events")]
        [SerializeField]
        [Space]
        protected UnityEvent<State<T0>> _stateChanged;

        #endregion

        #region Fields

        /// <summary>
        /// The states attached to the machines GameObject
        /// </summary>
        protected List<State<T0>> _states;

        protected State<T0> _currentState;
        protected State<T0> _previousState;

        #endregion

        #region Getters

        /// <summary>
        /// The machine's actor instance. This is set in the Machine's SetUp method.
        /// </summary>
        public T0 actor => _actor;

        /// <summary>
        /// A getter for the machine's Status
        /// </summary>
        public MachineStatus status => _status;

        /// <summary>
        /// This is the current active state for the this State Machine
        /// </summary>
        public State<T0> currentState => _currentState;

        /// <summary>
        /// This is the immediate previous state the machine was in.
        /// </summary>
        public State<T0> previousState => _previousState;

        /// <summary>
        /// Getter for the machine's default state
        /// </summary>
        public State<T0> defaultState => _defaultState;

        /// <summary>
        /// If CurrentStateName should be shown in the inspector
        /// </summary>
        protected bool showCurrentState => status == MachineStatus.On || status == MachineStatus.Paused;

        /// <summary>
        /// Indicates that the machine has no recognized states
        /// </summary>
        protected bool noRecognizedStates => _recognizedStates.Count == 0;

        #endregion

        #region Inspector Methods

        [Button]
        public void RecognizeStates()
        {
            _recognizedStates = GetComponents<State<T0>>().ToList();
        }

        [Button]
        public void ClearRecognizedStates()
        {
            _recognizedStates?.Clear();
        }

        #endregion

        #region Getters

        public UnityEvent<State<T0>> stateChanged => _stateChanged;

        #endregion

        #region Machine Engine

        /// <summary>
        /// Should be called in the actor's Awake();
        /// </summary>
        /// <param name="actor"> The machine's actor </param>
        public virtual void SetUp(T0 actor)
        {
            _status = MachineStatus.Off;

            this._actor = actor;

            _states = GetComponents<State<T0>>().ToList();

            if (_states.Count() == 0)
                Log.Danger($"{name} - There are no states attached to {_actor.GetType()}'s StateMachine.");

            _status = MachineStatus.Loading;
            LoadStates();
            StartMachine();
        }

        /// <summary>
        /// Loads Up all components of type State into a state list called states.
        /// Foreach loaded state its OnLoad() method will be fired.
        /// </summary>
        protected virtual void LoadStates()
        {
            if (_status != MachineStatus.Loading)
            {
                Log.Warning($"{name} - Machine for {_actor.GetType()} tryed loading its states out of time.");
                return;
            }

            foreach (State<T0> state in _states)
            {
                state.InternalLoad();
                state.OnLoadAction?.Invoke();
                state.SortTransitions();
            }

            _status = MachineStatus.Ready;
        }

        /// <summary>
        /// Should be called upon the machine's actor Start().
        /// </summary>
        public virtual void StartMachine()
        {
            if (_status != MachineStatus.Ready)
            {
                Log.Danger($"{name} - Machine for {_actor.GetType()} tryed to start but is not ready yet.");
                return;
            }

            Resume();
            RequestStateChange(_defaultState);
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Resume()
        {
            _status = MachineStatus.On;
        }

        /// <summary>
        /// Pauses the machine
        /// </summary>
        public virtual void Pause()
        {
            _status = MachineStatus.Paused;
        }

        /// <summary>
        /// Stops the machine
        /// </summary>
        public virtual void Stop()
        {
            _status = MachineStatus.Off;
        }

        #endregion

        #region Machine's Logic

        /// <summary>
        /// Defines a given state as active
        /// </summary>
        /// <param name="state"> The state to be set as active </param>
        /// <param name="forceInterruption"> If an uninterruptible state should be interrupted </param>
        public virtual void RequestStateChange(State<T0> state, bool forceInterruption = false)
        {
            if (_status != MachineStatus.On) return;

            if (_currentState != null && !_currentState.interruptible && !forceInterruption) return; // State cannot be interrupted, but will if forced.

            ChangeState(state);
        }

        /// <summary>
        /// Ends a state transitioning it into the target state.
        /// Case no target is provided, trasitions into the default state.
        /// </summary>
        /// <param name="target"> The state to be set as active </param>
        public virtual void EndState(State<T0> target = null)
        {
            if (_status != MachineStatus.On) return;

            if (target != null)
            {
                ChangeState(target);
            }
            else
            {
                ChangeState(_defaultState);
            }

        }

        protected virtual void ChangeState(State<T0> state)
        {

            if (state == _currentState || state == null) return; // Should not change 

            _previousState = _currentState; // Defines the previous state

            _currentState?.OnExitAction?.Invoke(); // Exiting current state

            _currentState = state; // Changing current state

            _currentState.OnEnterAction?.Invoke(); // Initializing new state

            _currentStateName = currentState.name;

            _stateChanged.Invoke(currentState);
        }


        /// <summary>
        /// Evaluates if the state should be transitioned. 
        /// If so, executes the transitation.
        /// </summary>
        protected virtual void HandleTick()
        {
            State<T0> state = EvaluateNextState();
            RequestStateChange(state);
        }

        /// <summary>
        /// Returns a state that has been evaluated as true on it's transition's condition
        /// </summary>
        protected virtual State<T0> EvaluateNextState()
        {
            foreach (StateTransition<T0> transition in currentState?.transitions)
            {
                if (transition.Condition())
                    return transition.state;
            }

            return null;
        }

        /// <summary>
        /// Sets a default state for the Machine. The given state will be used as a starting state
        /// and also as a fallback state.
        /// </summary>
        /// <typeparam name="T0"> The default state's type </typeparam>
        public virtual void SetDefaultState<T>() where T : State<T0>
        {
            _defaultState = GetComponent<T>();
        }

        #endregion

        #region Ticks

        /// <summary>
        /// Must be executed every Actor's monobehaviour Update() 
        /// </summary>
        public virtual void Tick()
        {
            if (_status != MachineStatus.On) return;

            HandleTick();
            currentState?.TickAction?.Invoke();
        }

        /// <summary>
        /// Can be executed every Actor's monobehaviour LateUpdate() 
        /// </summary>
        public virtual void LateTick()
        {
            if (_status != MachineStatus.On) return;

            HandleTick();
            currentState?.LateTickAction?.Invoke();
        }

        /// <summary>
        /// Must be executed every Actor's monobehaviour fixedUpdate
        /// </summary>
        public virtual void FixedTick()
        {
            if (_status != MachineStatus.On) return;

            HandleTick();
            currentState?.FixedTickAction?.Invoke();
        }

        #endregion

        #region Hierarchy

        #endregion
    }

    /// <summary>
    /// The machine's Statuses.
    /// </summary>
    public enum MachineStatus
    {
        Off,
        Loading,
        Ready,
        On,
        Paused,
    }
}
