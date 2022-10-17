using H2DT.NaughtyAttributes;
using UnityEngine;

namespace H2DT.Capabilities
{
    public abstract class LearnableAbilityScriptable<T> : ScriptableObject where T : Learnable
    {
        #region Editor

        [Required("Without a proper setup this component won't work.")]
        [SerializeField]
        private T _setup;

        #endregion

        #region Getters

        public T setup => _setup;

        #endregion

        #region Setup Stuff

        public virtual void ChangeSetup(T newSetup)
        {
            _setup = newSetup;
        }

        #endregion
    }
}
