using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Actions.FSM
{
    public abstract class MonolessState<T0>
    {
        #region Fields

        private string _name;
        private bool _interruptible = false;

        /// <summary>
        /// The state machine. Set inside InternalLoad() method wich is called by the machine.
        /// </summary>
        protected MonolessStateMachine<T0> _machine;

        // Actions
        public UnityAction LoadAction { get; private set; }
        public UnityAction OnEnterAction { get; private set; }
        public UnityAction OnExitAction { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Either the state name defined on Inspector or the type of the state class
        /// </summary>
        public string name => !string.IsNullOrEmpty(_name) ? _name : GetType().ToString();

        public bool interruptible { get => _interruptible; set => _interruptible = value; }

        #endregion

        #region Getters

        public T0 actor => _machine.actor;

        #endregion

        #region Loading the machine

        /// <summary>
        /// This will be called before the Load() method.
        /// </summary>
        public virtual void InternalLoad(MonolessStateMachine<T0> stateMachine)
        {
            _machine = stateMachine;
            LoadActions();
        }

        /// <summary>
        /// Loads methods as actions to be called during the state's lifecycle.
        /// </summary>
        protected virtual void LoadActions()
        {

            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("Load", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                LoadAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("OnEnter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnEnterAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

            mi = stateType.GetMethod("OnExit", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
                OnExitAction = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

        }

        #endregion

        #region Utils

        public void SetName(string newName)
        {
            _name = newName;
        }

        protected T GetMachine<T>() where T : MonolessStateMachine<T0>
        {
            return _machine as T;
        }

        #endregion
    }
}
