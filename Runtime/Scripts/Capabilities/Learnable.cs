using System.Collections;
using System.Collections.Generic;
using H2DT.Debugging;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities
{
    public abstract class Learnable : ScriptableObject
    {

        #region Editor

        [Header("Learnable")]
        [Tooltip("If the ability is learned (installed)")]
        [Space]
        [SerializeField]
        protected bool _learned;

        [SerializeField]
        [Tooltip("If the ability currently active")]
        protected bool _active;

        [SerializeField]
        [Space]
        protected bool _debugOn = false;

        [Header("Learnable Events")]
        [Space]
        [SerializeField]
        private UnityEvent<bool> _learnedStatusUpdate;

        [SerializeField]
        private UnityEvent<bool> _activationStatusUpdate;

        #endregion

        #region Getters

        public bool learned => _learned;
        public bool active => _active;

        public UnityEvent<bool> learnedStatusUpdate => _learnedStatusUpdate;
        public UnityEvent<bool> activationStatusUpdate => _activationStatusUpdate;

        #endregion

        #region Logic 

        /// <summary>
        /// Sets ability learned status as true and invokes learned status update event
        /// </summary>
        public virtual void Learn()
        {
            _learned = true;
            _learnedStatusUpdate.Invoke(_learned);
            DebugLearnable($"{GetType().Name} learn status update to {_learned}");
        }

        /// <summary>
        /// Sets ability learned status as false and invokes learned status update event.
        /// This also calls Deactivate() to ensure the ability is not active.
        /// </summary>
        public virtual void Unlearn()
        {
            _learned = false;
            _learnedStatusUpdate.Invoke(_learned);
            DebugLearnable($"{GetType().Name} learn status update to {_learned}");
            Deactivate();
        }

        /// <summary>
        /// If Ability is learned, activates it and invokes active status update event.
        /// </summary>
        /// <returns></returns>
        public virtual void Activate()
        {
            if (!_learned) return;

            _active = true;
            _activationStatusUpdate.Invoke(_active);
            DebugLearnable($"{GetType().Name} active status update to {_active}");
        }

        /// <summary>
        /// Deactivates the ability and invokes active status update event.
        /// </summary>
        public virtual void Deactivate()
        {
            _active = false;
            _activationStatusUpdate.Invoke(_active);
            DebugLearnable($"{GetType().Name} active status update to {_active}");
        }

        /// <summary>
        /// Learn and activate the ability.
        /// </summary>
        public virtual void LearnAndActivate()
        {
            Learn();
            Activate();
        }

        #endregion

        #region Debug 

        protected void DebugLearnable(string message)
        {
            if (!_debugOn) return;
            Log.Message(message);
        }

        #endregion

    }
}
