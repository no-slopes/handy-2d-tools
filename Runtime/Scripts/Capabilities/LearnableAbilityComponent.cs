using System.Collections;
using System.Collections.Generic;
using H2DT.NaughtyAttributes;
using UnityEngine;
using H2DT.Debugging;

namespace H2DT.Capabilities
{
    [DefaultExecutionOrder(300)]
    public abstract class LearnableAbilityComponent<T> : HandyComponent where T : Learnable
    {
        #region Editor

        [Required("Without a proper setup this component won't work.")]
        [SerializeField]
        private T _setup;

        #endregion

        #region Getters

        public T setup => _setup;

        #endregion

        #region Mono 

        protected virtual void Awake()
        {
            if (_setup == null)
            {
                Log.Danger($"{GetType().Name} setup is null. Please assign a proper setup to this ability.");
            }
        }

        #endregion

        #region Setup Stuff

        public virtual void ChangeSetup(T newSetup)
        {
            _setup = newSetup;
        }

        #endregion
    }
}
