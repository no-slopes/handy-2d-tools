
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Utils
{
    public abstract class ScriptableDispatcher<T> : ScriptableObject
    {
        #region Inspector

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<T> Dispatched;

        #endregion

        #region Logic

        public void Dispatch(T dispatchable)
        {
            Dispatched.Invoke(dispatchable);
        }

        #endregion
    }
}