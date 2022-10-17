using UnityEngine;
using H2DT.Debugging;
namespace H2DT.SpriteAnimations.Handlers
{
    public abstract class SpriteAnimationHandler
    {

        #region Fields

        protected SpriteAnimation _currentAnimation;

        protected SpriteAnimator _animator;

        /// <summary>
        /// List of frames used by the current cycle  
        /// </summary>
        protected SpriteAnimationCycle _currentCycle = new SpriteAnimationCycle();

        /// <summary>
        /// Used when running an animation
        /// </summary>
        protected float _currentCycleElapsedTime = 0.0f;

        /// <summary>
        /// Current frame of the current cycle
        /// </summary>
        protected SpriteAnimationFrame _currentFrame;

        /// <summary>
        /// If the animation has reached its end and should stop playing
        /// </summary>
        protected bool _animationEnded = false;

        #endregion       

        #region Properties  

        /// <summary>
        /// The duration in seconds a frame should display while in that animation
        /// </summary>
        protected float FrameDuration => _currentAnimation != null ? 1f / _currentAnimation.FPS : 0f;

        /// <summary>
        /// The duration of the current animation's cycle in seconds
        /// </summary>
        protected float CurrentCycleDuration => FrameDuration * _currentCycle.FrameCount;

        /// <summary>
        /// The index of the current frame in the current cycle.
        /// This is calculated based on the total amount of frames the current cycle has and the current elapsed time for 
        /// the that cycle.
        /// </summary>
        protected int CurrentFrameIndex => Mathf.FloorToInt(_currentCycleElapsedTime * _currentCycle.FrameCount / CurrentCycleDuration);

        /// <summary>
        /// If the animation has frames to be played
        /// </summary>
        protected bool HasCurrentFrames => _currentCycle != null && _currentCycle.FrameCount > 0;

        #endregion

        #region Getters 

        public SpriteAnimationCycle CurrentCycle => _currentCycle;

        #endregion

        #region Logic       

        /// <summary>
        /// Validates if the animation should be played.
        /// </summary>
        /// <returns> true if animation can be played </returns>
        protected bool ValidateAnimation(SpriteAnimation animation)
        {

            if (animation != null && animation.AllFrames != null && animation.AllFrames.Count > 0) return true;

            if (animation == null)
            {
                Log.Danger($"Sprite animation for {_animator.gameObject.name} - Trying to set null as current animation.");
                return false;
            }

            Log.Danger($"Sprite animation for {_animator.gameObject.name} - Could not evaluate the animation {GetType().Name} to be played. Did you set animation frames?");

            return false;
        }

        /// <summary>
        /// Sets the animator for this handler
        /// </summary>
        /// <param name="animator"></param>
        public void SetAnimator(SpriteAnimator animator)
        {
            _animator = animator;
        }

        #endregion

        #region Abstract Methods

        public abstract SpriteAnimationFrame EvaluateFrame(float deltaTime);
        public abstract void StartAnimation(SpriteAnimation animation);
        public abstract void StopAnimation();
        protected bool HasCycle { get; }

        #endregion

    }

}
