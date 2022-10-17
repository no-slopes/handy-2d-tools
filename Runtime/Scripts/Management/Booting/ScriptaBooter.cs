using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Booting
{
    [CreateAssetMenu(fileName = "New ScriptaBooter", menuName = "Handy 2D Tools/Management/Booting/ScriptaBooter")]
    public class ScriptaBooter : ScriptableObject, IBootable
    {
        #region Inspector

        [Space]
        [SerializeField]
        private List<ScriptableObject> _bootableObjects = new List<ScriptableObject>();

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent _bootComplete;

        [SerializeField]
        public UnityEvent _dismissComplete;

        #endregion

        #region Fields

        protected bool _booted = false;
        protected bool _dismissed = false;

        #endregion

        #region Getters

        public bool booted => _booted;
        public bool dismissed => _dismissed;

        // events
        public UnityEvent bootComplete => _bootComplete;
        public UnityEvent dismissComplete => _dismissComplete;

        #endregion

        #region Logic

        public virtual async Task BootableBoot()
        {
            List<Task> bootTasks = new List<Task>();

            foreach (ScriptableObject bootableObject in _bootableObjects)
            {
                if (bootableObject == null)
                {
                    Debug.LogWarning($"{name} - {GetType().Name} - Null bootable in list", this);
                }

                IBootable bootable = (IBootable)bootableObject;

                if (bootable == null)
                {
                    Debug.LogWarning($"{name} - {GetType().Name} - The {bootableObject.name} object is not Bootable", this);
                    continue;
                }

                bootTasks.Add(bootable.BootableBoot());
            }

            await Task.WhenAll(bootTasks);

            _booted = true;

            _bootComplete.Invoke();
            _bootComplete.RemoveAllListeners();
        }

        public virtual async Task BootableDismiss()
        {
            List<Task> bootTasks = new List<Task>();

            foreach (ScriptableObject bootableObject in _bootableObjects)
            {
                if (bootableObject == null)
                {
                    Debug.LogWarning($"{name} - {GetType().Name} - Null bootable in list", this);
                }

                IBootable bootable = (IBootable)bootableObject;

                if (bootable == null)
                {
                    Debug.LogWarning($"{name} - {GetType().Name} - The {bootableObject.name} object is not Bootable", this);
                    continue;
                }

                bootTasks.Add(bootable.BootableDismiss());
            }

            await Task.WhenAll(bootTasks);

            _dismissed = true;

            _dismissComplete.Invoke();
            _dismissComplete.RemoveAllListeners();
        }

        #endregion
    }
}
