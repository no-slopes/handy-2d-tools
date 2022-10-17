using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Debugging;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Generics.Transitions
{
    public class TransitionCommander<T> where T : ITransitionPerformer
    {
        #region Actions

        #endregion

        #region Logic

        /// <summary>
        /// Starts all enter transitions from all registered performers and calls the OnAllEnterPlayed action when they are all done.
        /// </summary>
        /// <param name="OnAllEnterPlayed"></param>
        public async Task PlayEnterTransitions(List<T> transitionPerformers)
        {
            if (transitionPerformers == null || transitionPerformers.Count <= 0) return;

            List<Task> transitions = new List<Task>();

            foreach (T transitionPerformer in transitionPerformers)
            {
                transitions.Add(transitionPerformer.PlayEnterTransition());
            }

            await Task.WhenAll(transitions);
        }

        /// <summary>
        /// Starts all exit transitions from all registered performers and calls the OnAllExitPlayed action when they all done.
        /// </summary>
        /// <param name="OnAllExitPlayed"></param>
        public async Task PlayExitTransitions(List<T> transitionPerformers)
        {
            if (transitionPerformers == null || transitionPerformers.Count <= 0) return;

            List<Task> transitions = new List<Task>();

            foreach (T transitionPerformer in transitionPerformers)
            {
                transitions.Add(transitionPerformer.PlayExitTransition());
            }

            await Task.WhenAll(transitions);
        }

        #endregion
    }
}
