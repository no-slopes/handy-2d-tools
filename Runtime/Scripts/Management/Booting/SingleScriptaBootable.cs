using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System;

namespace H2DT.Management.Booting
{
    public abstract class SingleScriptaBootable<T0> : ScriptableObject, IBootable where T0 : ScriptableObject
    {
        private static T0 _instance;

        public static T0 Instance
        {
            get => _instance;
        }

        #region Properties

        protected static T0 instance => _instance;

        #endregion

        #region Logic

        public virtual Task BootableBoot()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"{name} - {GetType().Name} - Trying to boot an already existent instance.", this);
                Debug.LogWarning($"Existent instance object: {Instance.name}", this);
                return Task.CompletedTask;
            }

            _instance = this as T0;

            return Task.CompletedTask;
        }

        public virtual Task BootableDismiss()
        {
            _instance = null;
            return Task.CompletedTask;
        }

        #endregion
    }
}
