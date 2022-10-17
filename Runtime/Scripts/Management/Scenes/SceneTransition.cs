using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Scenes
{
    public abstract class SceneTransition : HandyComponent
    {
        protected static string OpenTriggerName = "Open";
        protected static string CloseTriggerName = "Close";

        #region Fields

        protected TransitionCommander<ITransitionPerformer> _transitionCommander;
        protected List<ITransitionPerformer> _childrenTransitionPerformers;

        #endregion

        #region Actions

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);
            _childrenTransitionPerformers = GetComponentsInChildren<ITransitionPerformer>().ToList();
            _transitionCommander = new TransitionCommander<ITransitionPerformer>();
        }

        #endregion

        #region Transition Methods

        public virtual void Dismiss()
        {
            Destroy(gameObject);
        }

        public virtual async Task CloseCurtains()
        {
            await PerformCurtainsClosing();
            await _transitionCommander.PlayEnterTransitions(_childrenTransitionPerformers);
        }

        public virtual async Task OpenCurtains()
        {
            await _transitionCommander.PlayExitTransitions(_childrenTransitionPerformers);
            await PerformCurtainsOpening();
        }

        #endregion

        #region Internal Callbacks

        protected abstract Task PerformCurtainsOpening();
        protected abstract Task PerformCurtainsClosing();

        #endregion
    }
}
