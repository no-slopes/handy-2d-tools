using System.Linq;

namespace H2DT.SpriteAnimations.Handlers
{
    public class SimpleSpriteAnimationHandler : SpriteAnimationHandler
    {
        #region Properties 

        protected SimpleSpriteAnimation CurrentSimpleAnimation => _currentAnimation as SimpleSpriteAnimation;

        #endregion       

        #region Sprite Animation Logic 

        /// <summary>
        /// Must be called to start playing an animation
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {
            ValidateAnimation(animation);
            _currentAnimation = animation;

            ResetCycle();

            _animationEnded = false;
            _currentCycle = CurrentSimpleAnimation.Cycle;
        }

        /// <summary>
        /// Must be called every time the animation should be stopped.
        /// </summary>
        public override void StopAnimation()
        {
            EndAnimation();
            _currentAnimation = null;
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
            if (_animationEnded) return _currentFrame;

            _currentCycleElapsedTime += deltaTime;

            HandleCycles();

            _currentFrame = EvaluateCycleFrame();

            return _currentFrame;
        }

        #endregion

        #region Simple Sprite Animation Logic


        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {
            if (_currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
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
            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == _currentFrame) return _currentFrame;

            return evaluatedFrame;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle. In case the animation is a loop, it restarts the cycle.
        /// Case the animation is not a loop, it ends the animation.
        /// </summary>
        public void EndCycle()
        {
            _animator?.AnimationCycleEnded.Invoke(_currentAnimation, SpriteAnimationCompositeCycleType.Core);

            if (CurrentSimpleAnimation.Loop)
            {
                ResetCycle();
            }
            else
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
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _animationEnded = true;
        }

    }

}