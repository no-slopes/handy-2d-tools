using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Scenes
{
    public class CloseAndLoadTransition : SceneTransition
    {
        protected static string LoadStartTriggerName = "Start";
        protected static string LoadDismissTriggerName = "Dismiss";

        #region Inspector

        [Header("Animation")]
        [Space]
        [SerializeField]
        protected Animator _animator;

        [SerializeField]
        protected Animator _loadingImageAnimator;

        [SerializeField]
        protected Animator _loadingTextAnimator;

        #endregion

        #region Fields        

        protected TaskCompletionSource<bool> _onCloseCurtainsComplete;
        protected TaskCompletionSource<bool> _onOpenCurtainsComplete;

        #endregion

        #region Properties

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

        public void OnCloseCurtainsPerformed()// Called back by animation event
        {
            _onCloseCurtainsComplete.TrySetResult(true); // Tells the PerformCurtainsClosing that the tasks is done so it can move on after await
        }

        public void OnOpenCurtainsPerformed()// Called back by animation event
        {
            _onOpenCurtainsComplete.TrySetResult(true); // Tells the PerformCurtainsOpening that the tasks is done so it can move on after await
        }

        #endregion
    }
}
