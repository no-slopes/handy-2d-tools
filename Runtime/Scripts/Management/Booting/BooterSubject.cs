using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Booting
{
    public class BooterSubject : HandyComponent
    {
        #region Inspector

        [Header("Bootable Subject")]
        [Space]
        [SerializeField]
        private ScriptaBooter _booter;

        #endregion

        #region Fields

        private UnityAction _bootCompleted;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (_booter == null)
            {
                Debug.LogError($"Null booter", this);
                return;
            }

            LoadActions();

            if (!_booter.booted)
            {
                enabled = false;
                _booter.bootComplete.AddListener(OnBooterBoot);
                return;
            }

            _bootCompleted?.Invoke();
        }

        #endregion

        #region Reflection

        /// <summary>
        /// Loads methods as actions to be called during the state's lifecycle.
        /// </summary>
        protected virtual void LoadActions()
        {
            System.Type stateType = this.GetType();
            MethodInfo mi;

            mi = stateType.GetMethod("OnBootCompleted", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (mi != null)
                _bootCompleted = Delegate.CreateDelegate(typeof(UnityAction), this, mi) as UnityAction;

        }

        #endregion

        #region Booter Callbacks

        private void OnBooterBoot()
        {
            enabled = true;
            _bootCompleted?.Invoke();
            _booter.bootComplete.RemoveListener(OnBooterBoot);
        }

        #endregion
    }
}