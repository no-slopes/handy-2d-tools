using System;

namespace H2DT.Actions.FSM
{
    public class StateTransition<T0>
    {
        #region Properties

        /// <summary>
        /// The condition wich evaluates if transition should be made
        /// </summary>
        public Func<bool> Condition { get; protected set; }

        public State<T0> state { get; protected set; }

        #endregion

        /// <summary>
        /// The transition priority level
        /// </summary>
        public int priority { get; protected set; }

        public StateTransition(Func<bool> Condition, State<T0> target, int priority = 0)
        {
            this.Condition = Condition;
            this.state = target;
            this.priority = priority;
        }

    }
}