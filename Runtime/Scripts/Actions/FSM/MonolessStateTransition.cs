using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.Actions.FSM
{
    public class MonolessStateTransition<T0>
    {
        #region Fields

        private Func<bool> _Condition;
        private MonolessState<T0> _targetState;

        #endregion

        #region Getters

        public Func<bool> Condition;
        public MonolessState<T0> targetState;

        #endregion

        #region Constructors

        public MonolessStateTransition(Func<bool> TransitionCondition, MonolessState<T0> state)
        {
            _Condition = TransitionCondition;
            _targetState = state;
        }

        #endregion
    }
}
