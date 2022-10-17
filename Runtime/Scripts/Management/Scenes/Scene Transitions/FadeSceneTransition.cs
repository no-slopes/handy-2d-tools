using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Scenes
{
    public class FadeSceneTransition : SceneTransition
    {

        #region Inspector

        [Header("Animation")]
        [Space]
        [SerializeField]
        protected Animator _animator;

        #endregion

        #region Fields

        protected TaskCompletionSource<bool> _onCloseCurtainsComplete;
        protected TaskCompletionSource<bool> _onOpenCurtainsComplete;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
        }

        #endregion

        #region Transition Methods

        protected override async Task PerformCurtainsClosing()
        {

            _animator.SetTrigger(CloseTriggerName);
            _onCloseCurtainsComplete = new TaskCompletionSource<bool>();

            await _onCloseCurtainsComplete.Task;
        }

        protected override async Task PerformCurtainsOpening()
        {
            _animator.SetTrigger(OpenTriggerName);
            _onOpenCurtainsComplete = new TaskCompletionSource<bool>();

            await _onOpenCurtainsComplete.Task;
        }

        #endregion

        #region Animation Events callbacks

        public void OnCloseCurtainsPerformed()
        {
            _onCloseCurtainsComplete.TrySetResult(true);
        }

        public void OnOpenCurtainsPerformed()
        {
            _onOpenCurtainsComplete.TrySetResult(true);
        }

        #endregion
    }
}
