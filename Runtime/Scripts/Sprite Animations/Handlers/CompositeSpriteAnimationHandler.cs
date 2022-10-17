using System.Linq;

namespace H2DT.SpriteAnimations.Handlers
{
    public class CompositeSpriteAnimationHandler : SpriteAnimationHandler
    {
        #region Fields


        /// <summary>
        /// The animation cycle that is currently playing
        /// </summary>
        protected SpriteAnimationCompositeCycleType _currentCycleType = SpriteAnimationCompositeCycleType.None;

        #endregion

        #region Properties  

        protected CompositeSpriteAnimation CurrentCompositeAnimation => _currentAnimation as CompositeSpriteAnimation;

        /// <summary>
        /// True if the animation has any frame configured
        /// </summary>
        protected bool HasFrames => CurrentCompositeAnimation.AllFrames != null && CurrentCompositeAnimation.AllFrames.Count > 0;

        #endregion

        #region Animation Logic 

        /// <summary>
        /// Must be called every time the animation is started
        /// </summary>
        public override void StartAnimation(SpriteAnimation animation)
        {

            ValidateAnimation(animation);

            _currentAnimation = animation;
            _animationEnded = false;

            ChangeCycle(EvaluateFirstCycle());
        }

        /// <summary>
        /// Must be called every time the animation is stopped
        /// </summary>
        public override void StopAnimation()
        {
            if (_currentAnimation == null || _animationEnded) return;
            EndAnimation();
        }

        /// <summary>
        /// Evaluates what sprite should be displayed based on the current cycle.
        /// This also handles the animation cycles.
        /// This MUST be called every LateUpdate()
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns> The evaluated sprite </returns>
        public override SpriteAnimationFrame EvaluateFrame(float deltaTime)
        {
            if (_currentAnimation == null || _animationEnded || _currentCycleType == SpriteAnimationCompositeCycleType.None) return null;

            _currentCycleElapsedTime += deltaTime;

            HandleCycles();

            _currentFrame = EvaluateCycleFrame();

            return _currentFrame;
        }

        #endregion

        #region Composite Animation Logic 

        /// <summary>
        /// Handles the animation cycles. 
        /// It evaluates if the current cycle is over and if so, it changes the cycle.
        /// This also evaluate what is the current frame of the current cycle.
        /// </summary>
        protected void HandleCycles()
        {
            if (_currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                EndCycle(_currentCycleType);

                SpriteAnimationCompositeCycleType nextCycle = EvaluateNextCycle();

                // Debug.Log($"{currentCycle} ended");
                // Debug.Log($"Duration: {cycleDuration} | CurrentTime: {cycleCurrentTime}");
                // Debug.Log($"CurrentFrameIndex: {currentFrameIndex}");

                if (nextCycle != SpriteAnimationCompositeCycleType.None)
                {
                    ChangeCycle(nextCycle);
                }
                else
                {
                    EndAnimation();
                    return;
                }
            }
        }

        /// <summary>
        /// Evaluates what should be used as the animation's first cycle.
        /// </summary>
        /// <returns> The evaluated cycle </returns>
        protected SpriteAnimationCompositeCycleType EvaluateFirstCycle()
        {
            if (CurrentCompositeAnimation.HasAntecipation)
            {
                return SpriteAnimationCompositeCycleType.Antecipation;
            }

            return SpriteAnimationCompositeCycleType.Core;
        }

        /// <summary>
        /// Evaluates what should be the animation's next cycle based on its current cycle
        /// </summary>
        /// <returns> The evaluated cycle </returns>
        protected SpriteAnimationCompositeCycleType EvaluateNextCycle()
        {
            if (_currentCycleType == SpriteAnimationCompositeCycleType.Antecipation)
            {
                return SpriteAnimationCompositeCycleType.Core;
            }

            if (_currentCycleType == SpriteAnimationCompositeCycleType.Core)
            {
                if (CurrentCompositeAnimation.LoopableCore) return SpriteAnimationCompositeCycleType.Core;

                return SpriteAnimationCompositeCycleType.Recovery;
            }

            if (_currentCycleType == SpriteAnimationCompositeCycleType.Recovery)
            {
                return SpriteAnimationCompositeCycleType.None;
            }

            return SpriteAnimationCompositeCycleType.None;
        }

        /// <summary>
        /// This changes the current cycle and resets the cycle's elapsed time and currentFrame.
        /// </summary>
        /// <param name="cycleType"></param>
        protected void ChangeCycle(SpriteAnimationCompositeCycleType cycleType)
        {
            ResetCycle();

            if (_currentCycleType == SpriteAnimationCompositeCycleType.Core && (cycleType == SpriteAnimationCompositeCycleType.Core && CurrentCompositeAnimation.LoopableCore))
            {
                _currentCycle = CurrentCompositeAnimation.CoreCycle;
                return;
            }

            if (cycleType == _currentCycleType) return;

            _currentCycleType = cycleType;

            switch (_currentCycleType)
            {
                case SpriteAnimationCompositeCycleType.Antecipation:
                    _currentCycle = CurrentCompositeAnimation.AntecipationCycle;
                    break;
                case SpriteAnimationCompositeCycleType.Core:
                    _currentCycle = CurrentCompositeAnimation.CoreCycle;
                    break;
                case SpriteAnimationCompositeCycleType.Recovery:
                    _currentCycle = CurrentCompositeAnimation.RecoveryCycle;
                    break;
                    // case SpriteAnimationCycleType.None:
                    //     _currentCycle.Clear();
                    //     break;
            }
        }

        /// <summary>
        /// This evaluates and sets the current frame of the current cycle.
        /// </summary>
        /// <returns> The evaluated frame </returns>
        protected SpriteAnimationFrame EvaluateCycleFrame()
        {
            SpriteAnimationFrame evaluatedFrame = _currentCycle.Frames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null) return _currentFrame;

            return evaluatedFrame;
        }

        /// <summary>
        /// Call this to stop core loop. 
        /// In case the animation has recovery frames and the given playRecovery is true, it will enter the 
        /// recovery cycle. Otherwise, it will end the animation.
        /// </summary>
        /// <param name="playRecovery"></param>
        public void StopCoreLoop(bool playRecovery = true)
        {
            if (CurrentCompositeAnimation.HasRecovery && playRecovery)
            {
                ChangeCycle(SpriteAnimationCompositeCycleType.Recovery);
            }
            else
            {
                EndCycle(SpriteAnimationCompositeCycleType.Core);
                EndAnimation();
            }
        }

        /// <summary>
        /// Ends the animation at the current frame.
        /// </summary>
        protected void EndAnimation()
        {
            _animationEnded = true;
            _currentAnimation = null;
            _currentFrame = null;
            _currentCycleType = SpriteAnimationCompositeCycleType.None;
        }

        #endregion

        /// <summary>
        /// Ends the current cycle.
        /// In case the current cycle is the recovery cycle, it will end the animation.
        /// </summary>
        /// <param name="cycle"></param>
        public void EndCycle(SpriteAnimationCompositeCycleType cycle)
        {
            _animator?.AnimationCycleEnded.Invoke(_currentAnimation, _currentCycleType);

            if (cycle == SpriteAnimationCompositeCycleType.Recovery)
            {
                EndAnimation();
            }
        }

        /// <summary>
        /// Resets the cycle's elapsed time and currentFrame.
        /// </summary>
        protected void ResetCycle()
        {
            _currentCycleElapsedTime = 0f;
            _currentFrame = null;
        }
    }
}