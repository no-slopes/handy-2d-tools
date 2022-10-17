using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using H2DT.Debugging;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations.Handlers;

namespace H2DT.SpriteAnimations
{
    /// <summary>
    /// This compoment represents the animator for a sprite renderer.
    /// </summary>
    [DefaultExecutionOrder(-1100)]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : HandyComponent
    {
        [SerializeField]
        [ShowIf("playing")]
        [ReadOnly]
        protected SpriteAnimation _currentAnimation;

        [SerializeField]
        [Space]
        protected List<SpriteAnimation> _spriteAnimations = new List<SpriteAnimation>();

        [SerializeField]
        [Space]
        protected bool _playAutomatically = true;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animationChanged;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animatorPlaying;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animatorPaused;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation> _animatorStopped;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation, SpriteAnimationCompositeCycleType> _animationCycleEnded;

        [Foldout("Events")]
        [SerializeField]
        protected UnityEvent<SpriteAnimation, SpriteAnimationCycle> _comboAnimationCycleEnded;

        #region Components

        protected SpriteRenderer _spriteRenderer;

        #endregion

        #region Fields

        protected SpriteAnimationHandlerFactory _animationHandlerFactory;
        protected SpriteAnimationFrame _currentFrame;

        protected SpriteAnimationHandler _currentHandler;
        protected Dictionary<SpriteAnimationFrame, UnityEvent> _frameEvents = new Dictionary<SpriteAnimationFrame, UnityEvent>();

        #endregion

        #region Getters

        public SpriteAnimation defaultAnimation => _spriteAnimations.Count > 0 ? _spriteAnimations[0] : null;

        public List<SpriteAnimation> animations => _spriteAnimations;

        #endregion

        protected SpriteAnimatorState _state = SpriteAnimatorState.Stopped;

        public bool playing => _state == SpriteAnimatorState.Playing;
        public bool paused => _state == SpriteAnimatorState.Paused;
        public bool stopped => _state == SpriteAnimatorState.Stopped;

        #region Getters

        public SpriteAnimation currentAnimation => _currentAnimation;
        public SpriteAnimationFrame currentFrame => _currentFrame;

        public UnityEvent<SpriteAnimation> AnimatorPlaying => _animatorPlaying;
        public UnityEvent<SpriteAnimation> AnimatorPaused => _animatorPaused;
        public UnityEvent<SpriteAnimation> AnimatorStopped => _animatorStopped;
        public UnityEvent<SpriteAnimation> AnimationChanged => _animationChanged;
        public UnityEvent<SpriteAnimation, SpriteAnimationCompositeCycleType> AnimationCycleEnded => _animationCycleEnded;
        public UnityEvent<SpriteAnimation, SpriteAnimationCycle> ComboAnimationCycleEnded => _comboAnimationCycleEnded;

        #endregion

        #region Mono

        protected void Awake()
        {
            _animationHandlerFactory = new SpriteAnimationHandlerFactory(this);

            FindComponent<SpriteRenderer>(ref _spriteRenderer);

            if (_spriteRenderer == null) _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            if (_playAutomatically && defaultAnimation != null)
                Play(defaultAnimation);

        }

        protected void LateUpdate()
        {
            EvaluateAndChangeCurrentFrame();
        }

        #endregion

        #region Controlling Animator

        /// <summary>
        /// Plays the default animation. 
        /// </summary>
        public void Play()
        {
            if (_currentAnimation == null)
            {
                ChangeAnimation(defaultAnimation);
            }

            if (_currentAnimation != null)
                Play(_currentAnimation);
        }

        /// <summary>
        /// Plays the specified animation by its name. Note that the animation must be registered to the 
        /// animator in orther to be found.
        /// </summary>
        /// <param name="name"></param>
        public void Play(string animationName)
        {
            Play(GetAnimationByName(animationName));
        }

        /// <summary>
        /// Plays the given animation. Does not require registering.
        /// </summary>
        /// <param name="animation"></param>
        public void Play(SpriteAnimation animation)
        {
            if (animation == _currentAnimation) return;

            ChangeAnimation(animation);

            _state = SpriteAnimatorState.Playing;
        }

        /// <summary>
        /// Pauses the animator. Use Resume to continue.
        /// </summary>
        public void Pause()
        {
            _state = SpriteAnimatorState.Paused;
            AnimatorPaused?.Invoke(_currentAnimation);
        }

        /// <summary>
        /// Resumes a paused animator.
        /// </summary>
        public void Resume()
        {
            _state = SpriteAnimatorState.Playing;
            AnimatorPlaying.Invoke(_currentAnimation);
        }

        /// <summary>
        /// This changes the current animation to be played by the animator. It will call 
        /// the current animation Stop() method and the new animation Start() method.
        /// </summary>
        /// <param name="animation"></param>
        protected void ChangeAnimation(SpriteAnimation animation)
        {

            if (animation == null) // If the animation is null, prevent changing.
            {
                Log.Warning($"Sprite Animator for {gameObject.name} - Could not evaluate an animation to be played. Check animation passed as parameter and the animation frames.");
                return;
            }

            ClearFrameEvents(); // Clear frame events before changing animation.

            _currentHandler?.StopAnimation(); // Stop current animation.

            _currentAnimation = animation; // current animtion is now the given animation.

            _currentHandler = _animationHandlerFactory.GetHandler(_currentAnimation); // Sets the current handler to the given animation.

            _currentHandler.StartAnimation(_currentAnimation); // Starts the given animation.

            _animationChanged.Invoke(animation); // Fires the animation changed event.
        }

        public void PlayNextComboCycle()
        {
            if (_currentHandler.GetType() != typeof(ComboSpriteAnimationHandler)) return;

            (_currentHandler as ComboSpriteAnimationHandler).PlayNextCycle();
        }

        #endregion

        #region Handling Animation

        /// <summary>
        /// Returns an animation wich was registered to the animator based on given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SpriteAnimation GetAnimationByName(string name)
        {
            return _spriteAnimations.FirstOrDefault(a => a.Name == name);
        }

        /// <summary>
        /// This should be called every LateUpdate to evaluate the current animation and change the sprite.
        /// </summary>
        protected void EvaluateAndChangeCurrentFrame()
        {
            if (!playing) return;

            SpriteAnimationFrame frame = _currentHandler.EvaluateFrame(Time.deltaTime);

            if (frame == null || frame == _currentFrame) return;

            _spriteRenderer.sprite = frame.Sprite;

            _currentFrame = frame;

            FireFrameEvent(_currentFrame);
        }

        /// <summary>
        /// Sets the current handler to the given animation.
        /// </summary>
        /// <param name="animation"></param>
        protected void SetCurrentHandler(SpriteAnimation animation)
        {
            _currentHandler = _animationHandlerFactory.GetHandler(animation);
        }

        /// <summary>
        /// In the case the current animation is composite and its core is loopable you must call this 
        /// to stop the loop and make the animation proceed to recovery
        /// </summary>
        public void StopCompositeAnimationCoreLoop()
        {
            if (_currentHandler.GetType() != typeof(CompositeSpriteAnimationHandler)) return;

            (_currentHandler as CompositeSpriteAnimationHandler).StopCoreLoop();
        }

        #endregion

        #region Handling Frame Events

        /// <summary>
        /// Returns an Event for an specific frame. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(SpriteAnimationFrame frame)
        {
            if (!_frameEvents.ContainsKey(frame))
                _frameEvents.Add(frame, new UnityEvent());

            return _frameEvents[frame];
        }

        /// <summary>
        /// Returns an Event for an specific frame found using the given ID. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(int id)
        {
            SpriteAnimationFrame frame = _currentAnimation.AllFrames.FirstOrDefault(f => f.Id == id);

            if (frame == null) return null;

            return GetFrameEvent(frame);
        }

        /// <summary>
        /// Returns an Event for an specific frame found using the given string set on the animation's inspector. If the event does not exist, it will be created.
        /// Events will be discarded upon animation changes. So use this every time you set the given
        /// frame's animation.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UnityEvent GetFrameEvent(string name)
        {
            SpriteAnimationFrame frame = _currentAnimation.AllFrames.FirstOrDefault(f => f.Name == name);

            if (frame == null) return null;

            return GetFrameEvent(frame);
        }

        /// <summary>
        /// Used to clear all frame events preventing memory leaks.
        /// </summary>
        protected void ClearFrameEvents()
        {
            foreach (KeyValuePair<SpriteAnimationFrame, UnityEvent> frameEvent in _frameEvents)
            {
                frameEvent.Value.RemoveAllListeners();
            }

            _frameEvents.Clear();
        }

        /// <summary>
        /// Fires the frame event for the given frame.
        /// </summary>
        /// <param name="frame"></param>
        protected void FireFrameEvent(SpriteAnimationFrame frame)
        {
            if (!_frameEvents.ContainsKey(frame)) return;

            _frameEvents[frame].Invoke();
        }

    }

    #endregion

}