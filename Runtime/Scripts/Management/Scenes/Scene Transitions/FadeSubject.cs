using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Scenes
{
    public class FadeSubject : HandyComponent, ITransitionPerformer
    {
        protected static string FadeInTriggerName = "FadeIn";
        protected static string FadeOutTriggerName = "FadeOut";

        protected Animator _animator;
        protected bool _interrupted;

        protected TaskCompletionSource<bool> _onEnterTransitionComplete;
        protected TaskCompletionSource<bool> _onExitTransitionComplete;

        protected void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void OnFadeInComplete()
        {
            _onEnterTransitionComplete.SetResult(true);
        }

        void OnFadeOutComplete()
        {
            _onExitTransitionComplete.SetResult(true);
        }

        public async Task PlayEnterTransition()
        {
            _animator.SetTrigger(FadeInTriggerName);
            _onEnterTransitionComplete = new TaskCompletionSource<bool>();

            await _onEnterTransitionComplete.Task;
        }

        public async Task PlayExitTransition()
        {
            _animator.SetTrigger(FadeOutTriggerName);
            _onExitTransitionComplete = new TaskCompletionSource<bool>();

            await _onExitTransitionComplete.Task;
        }
    }
}
