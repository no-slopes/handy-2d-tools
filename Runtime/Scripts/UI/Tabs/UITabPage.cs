using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT;
using H2DT.Generics.Transitions;
using UnityEngine;

namespace HD2T.UI.Tabs
{
    public class UITabPage : HandyComponent
    {
        #region Fields

        protected bool _active;
        protected TransitionCommander<ITransitionPerformer> _transitionCommander = new TransitionCommander<ITransitionPerformer>();
        protected List<ITransitionPerformer> _transitionPerformers = new List<ITransitionPerformer>();

        #endregion

        #region Performers Handling

        protected void RegisterTransitionPerformer(ITransitionPerformer transitionPerformer)
        {
            if (_transitionPerformers.Contains(transitionPerformer)) return;
            _transitionPerformers.Add(transitionPerformer);
        }

        protected void UnregisterTransitionPerformer(ITransitionPerformer transitionPerformer)
        {
            _transitionPerformers.Remove(transitionPerformer);
        }

        #endregion

        #region Tasks

        public void ActivateSilently()
        {
            gameObject.SetActive(true);// Turn object on before executing transitions
            _active = true;
        }

        public async Task Activate()
        {
            gameObject.SetActive(true);// Turn object on before executing transitions
            await _transitionCommander.PlayEnterTransitions(_transitionPerformers);
            _active = true;
        }

        public async Task Deactivate()
        {
            await _transitionCommander.PlayExitTransitions(_transitionPerformers);
            gameObject.SetActive(false); // Turns object off only after executing transitions.
            _active = false;
        }

        #endregion
    }
}
