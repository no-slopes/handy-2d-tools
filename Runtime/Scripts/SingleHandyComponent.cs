
using System;
using UnityEngine;

namespace H2DT
{
    public class SingleHandyComponent<T0> : HandyComponent where T0 : Component
    {
        #region Static

        public static T0 Instance { get; private set; }

        #endregion

        #region Inspector

        [Header("Singleton")]
        [Space]
        [Tooltip("Mark this if you want this object to NOT be destroyed whe a new scene is loaded.")]
        [SerializeField]
        private bool _persistent = true;

        #endregion

        #region Fields

        private bool _singletonReady = false;

        #endregion

        #region Properties

        protected bool singletonReady => _singletonReady;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            LoadSingleton();
        }

        #endregion

        #region Singleton

        protected void LoadSingleton()
        {
            T0 currentInstance = Instance;
            T0 thisInstance = this as T0;

            if (currentInstance != null && currentInstance != thisInstance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = thisInstance;
            _singletonReady = true;

            if (_persistent)
                DontDestroyOnLoad(thisInstance);
        }

        #endregion
    }
}