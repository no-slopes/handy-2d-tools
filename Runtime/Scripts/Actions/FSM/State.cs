using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using H2DT.NaughtyAttributes;
using UnityEngine;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace H2DT.Actions.FSM
{
    /// <summary>
    /// Represents a State controlled by the StateMachine class.
    /// </summary>
    [DefaultExecutionOrder(200)]
    public abstract class State<T0> : HandyComponent
    {

        #region Fields

        [Header("State Configuration")]
        [Tooltip("Set this to get visual feedback on the Machine's component inspector")]
        [SerializeField]
        [Space]
        protected string _name;

        [Tooltip("In case this state can be interrupted, set this to true")]
        [SerializeField]
        protected bool _interruptible = false;

        /// <summary>
        /// The state machine. Set inside InternalLoad() method wich is called by the machine.
        /// </summary>
        protected StateMachine<T0> _machine;

        // Actions
        public UnityAction OnEnterAction { get; private set; }
        public UnityAction OnExitAction { get; private set; }
        public UnityAction OnLoadAction { get; private set; }
        public UnityAction TickAction { get; private set; }
        public UnityAction LateTickAction { get; private set; }
        public UnityAction FixedTickAction { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// List of transitions of this state.
        /// </summary>
        public List<StateTransition<T0>> transitions { get; protected set; } = new List<StateTransition<T0>>();

        /// <summary>
        /// Either the state name defined on Inspector or the type of the state class
        /// </summary>
        new public string name => !string.IsNullOrEmpty(_name) ? _name : GetType().ToString();

        public bool interruptible { get { return _interruptible; } set { _interruptible = value; } }

        #endregion

        #region Getters

        public T0 actor => _machine.actor;

        #endregion

        #region Transitions

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="Condition"> A bool returning callback wich evaluates if the state should become active or not </param>
        /// <param name="state"> The state wich should become active based on condition </param>
        /// <param name="priority"> Priority level </param>
        protected virtual void AddTransition(Func<bool> Condition, State<T0> state, int priority = 0)
        {
            StateTransition<T0> transition = new StateTransition<T0>(Condition, state, priority);
            AddTransition(transition);
        }

        /// <summary>
        /// Adds a transition into the available transitions of this state
        /// </summary>
        /// <param name="transition"> The State Trasitions to be add </param>
        protected virtual void AddTransition(StateTransition<T0> transition)
        {
            transitions.Add(transition);
        }

        /// <summary>
        /// Ends a state after a given time in seconds.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected async void EndAfterTimeAsync(float time, State<T0> target)
        {
            int convertedTime = (int)time * 1000; // turning into miliseconds
            await Task.Delay(convertedTime);

            _machine.EndState(target);
        }

        /// <summary>
        /// Ends a state after a given time in seconds.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected IEnumerator EndAfterTime(float time, State<T0> target)
        {

            yield return new WaitForSeconds(time);

            _machine.EndState(target);
        }

        #endregion

        #region Loading the machine

        /// <summary>
        /// This will be called before the Load() method.
        /// </summary>
        public virtual void InternalLoad()
        {
            _machine = GetComponent<StateMachine<T0>>();
            LoadActions();
        }

        /// <summary>
        /// Sorts transitions based on priority. Descending
        /// </summary>
        public virtual void SortTransitions()
        {
            transitions = transitions.OrderByDescending(transition => transition.priority).ToList();
        }

        /// <summary>
        /// Loads methods as actions to be called during the state's lifecycle.
        /// </summary>
        protected virtual void LoadActions()
        {

            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("OnEnter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnEnterAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("OnExit", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnExitAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("OnLoad", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnLoadAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("Tick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                TickAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("LateTick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                LateTickAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("FixedTick", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                FixedTickAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

        }

        #endregion

        #region Utils

        public void SetName(string newName)
        {
            _name = newName;
        }

        protected T GetMachine<T>() where T : StateMachine<T0>
        {
            return _machine as T;
        }

        #endregion
    }
}