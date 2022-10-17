using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    public abstract class Attack : DocumentedComponent
    {
        #region Inspector

        [SerializeField]
        protected UnityEvent<bool> _engagedOnAttack;

        #endregion

        #region Fields

        protected bool _engaged = false;
        protected bool _locked = false;

        #endregion

        #region Getters

        public bool engaged => _engaged;

        // Events

        public UnityEvent<bool> EngagedOnAttack => _engagedOnAttack;

        #endregion

        #region Abstractions

        protected abstract bool CanEngage { get; }

        #endregion

        #region Engagement Handling

        public virtual void Request()
        {
            if (!CanEngage) return;

            Engage();
        }

        public virtual void Engage()
        {
            _engaged = true;
            _engagedOnAttack.Invoke(true);
        }

        public virtual void Disengage()
        {
            _engaged = false;
            _engagedOnAttack.Invoke(false);
        }

        #endregion

        #region Locking

        public virtual void Lock(bool shouldLock)
        {
            _locked = shouldLock;
        }

        #endregion
    }
}
