using System.Linq;

namespace H2DT.SpriteAnimations.Handlers
{
    public class ComboSpriteAnimationHandler : SpriteAnimationHandler
    {
        #region Fields

        protected int _cyclesCount;
        protected int _currentCycleCounter;
        protected bool _cycleFreezed;

        #endregion

        #region Properties 

        protected ComboSpriteAnimation CurrentComboAnimation => _currentAnimation as ComboSpriteAnimation;

        #endregion       

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            ValidateAnimation(animation);

            _animationEnded = false;

            _currentAnimation = animation;
            _cyclesCount = CurrentComboAnimation.Cycles.Count;
            _currentCycleCounter = 0;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            EndAnimation();
        }

        /// <summary>
        /// Evaluates what sprite should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// This MUST be called every LateUpdate()
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public override SpriteAnimationFrame EvaluateFrame(float deltaTime)
        {
            if (_animationEnded) return null;

            _currentCycleElapsedTime += deltaTime;

            HandleCycles();

            _currentFrame = EvaluateCycleFrame();

            return _currentFrame;
        }

        #endregion

        #region Combo Sprite Animation Logic


        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {
            if (!_cycleFreezed && _currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                EndCycle();
            }
        }

        /// <summary>
        /// This evaluates and sets the current frame of the current cycle.
        /// </summary>
        /// <returns> The evaluated frame </returns>
        protected SpriteAnimationFrame EvaluateCycleFrame()
        {
            if (_cycleFreezed) return null;

            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return _currentFrame;

            return evaluatedFrame;
        }

        public void PlayNextCycle()
        {
            // current is last?
            if (_currentCycleCounter >= _cyclesCount) return; // there is no next cycle

            ResetCycle();
            _currentCycleCounter++;
            _currentCycle = CurrentComboAnimation.Cycles[_currentCycleCounter - 1];
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        protected void EndCycle()
        {
            _cycleFreezed = true;
            _animator.ComboAnimationCycleEnded.Invoke(_currentAnimation, _currentCycle);

            if (_currentCycleCounter >= _cyclesCount)
            {
                EndAnimation();
            }
        }

        /// <summary>
        /// Resets the cycle.
        /// </summary>
        public void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
            _cycleFreezed = false;
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _animationEnded = true;
            _currentAnimation = null;
        }

    }

}