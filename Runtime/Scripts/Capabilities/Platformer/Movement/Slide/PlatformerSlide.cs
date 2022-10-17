using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using H2DT.NaughtyAttributes;
using H2DT.Debugging;
using H2DT.Capabilities;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Movement/PlatformerSlide")]
    public class PlatformerSlide : LearnableAbilityComponent<PlatformerSlideSetup>
    {

        #region Editor

        [Header("Dependencies")]
        [InfoBox("If you prefer you can read the docs on how to feed this component directly through one of your scripts.")]
        [Space]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerSlideHandler you can mark this and it will subscribe to its events.")]
        [SerializeField]
        protected bool _seekSlideHandler = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerMovementPerformer you can mark this and it will subscribe to its events. DynamicPlatformerMovement, for example, implements it.")]
        [SerializeField]
        protected bool _seekMovementPerformer = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D, for example, implements it.")]
        [SerializeField]
        protected bool _seekGroundingProvider = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerHorizontalDirectionProvider you can mark this and it will subscribe to its events. HorizontalFlip, for example, implements it.")]
        [SerializeField]
        protected bool _seekHorizontalFacingDirectionProvider = false;

        [Header("Debug")]
        [Tooltip("Turn this on if you want to see ceiling detection")]
        [Space]
        [SerializeField]
        protected bool _debugOn;

        [ShowIf("_debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool _performing = false;

        [ShowIf("_debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool _locked = false;

        [Header("Config")]
        [Space]
        [Tooltip("The collider used to detect ceilings")]
        [SerializeField]
        protected Collider2D _slidingCollider;

        [Tooltip("Layers that should be considered ceiling")]
        [SerializeField]
        protected LayerMask _whatIsCeiling;

        [Tooltip("Whatever colliders that should be disabled while sliding")]
        [SerializeField]
        protected List<Collider2D> _collidersToDisable;

        [Tooltip("Ray cast length while performing to detect ceilings")]
        [SerializeField]
        protected float _ceilingDetectionLength = 2f;

        [Header("Perform Approach")]
        [InfoBox("If you uncheck this it means you will have to call the Perform() method inside the Physics Update of any component you create to handle this one.")]
        [Tooltip("In case you want to handle when and how the Perform() method is called, you should turn this off")]
        [SerializeField]
        [Space]
        protected bool _autoPerform = true;

        #endregion


        #region Components

        protected IPlatformerSlideHandler _slideHandler;
        protected IPlatformerMovementPerformer _movementPerformer;
        protected IPlaftormerGroundingProvider _groundingProvider;
        protected IPlatformerFacingDirectionProvider _horizontalDirectionProvider;

        #endregion

        #region Fields

        protected bool _grounded = false;
        protected float _slideStartedAt;
        protected float _canSlideAt;
        protected bool _slideLocked = false;
        protected float _currentSlideTimer = 0;
        protected float _currentFacingDirectionSign = 0;
        protected bool _stopingDueToLostGround = false;

        protected float _lengthConvertionRate = 100f;

        #endregion

        #region Properties

        protected bool canStartSliding => !_performing && _grounded && !_slideLocked && Time.fixedTime >= _canSlideAt;
        protected float lengthConverted => _ceilingDetectionLength / _lengthConvertionRate;

        public IPlatformerSlideHandler slideHandler { get { return _slideHandler; } set { _slideHandler = value; } }
        public IPlatformerMovementPerformer movementPerformer { get { return _movementPerformer; } set { _movementPerformer = value; } }
        public IPlaftormerGroundingProvider groundingProvider { get { return _groundingProvider; } set { _groundingProvider = value; } }
        public IPlatformerFacingDirectionProvider horizontalDirectionProvider { get { return _horizontalDirectionProvider; } set { _horizontalDirectionProvider = value; } }
        public LayerMask whatIsCeiling { get { return _whatIsCeiling; } set { _whatIsCeiling = value; } }

        #endregion

        #region Getters

        public bool performing => _performing;
        public bool locked => _locked;


        // Events
        public UnityEvent<bool> SlideUpdate => setup.SlideUpdate;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            FindComponents();
        }

        protected virtual void FixedUpdate()
        {
            if (!_autoPerform || !_performing) return;
            Perform();
        }

        protected virtual void OnEnable()
        {
            SubscribeSeekers();

            if (_slidingCollider == null)
                _slidingCollider = GetComponent<Collider2D>();

            if (_whatIsCeiling == 0)
                Log.Danger($"No ceiling defined for {GetType().Name}");
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Call this to request a Slide.
        /// </summary>
        /// <param name="directionSign"></param>
        public void Request()
        {
            if (!setup.active) return;
            if (!canStartSliding) return;
            StartSlide();
        }

        /// <summary>
        /// Starts the jump process so Ascend can be called each physics frame
        /// </summary>
        protected void StartSlide()
        {
            ToggleColliders(false);
            _currentSlideTimer = 0;
            _slideStartedAt = Time.fixedTime;

            _movementPerformer.StopMovement();

            _performing = true;
            _stopingDueToLostGround = false;
            SlideUpdate.Invoke(_performing);
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update.
        /// </summary>
        public void Perform()
        {
            if (!_performing) return;

            if (!_stopingDueToLostGround && setup.stopWhenNotGrounded && !_grounded)
            {
                _stopingDueToLostGround = true;
                StartCoroutine(StopAfterTime(0.01f));
            } // Stop if not grounded

            if (_currentSlideTimer > setup.duration && !IsUnderCeiling()) { Stop(); return; } // Stop only if duration is reached and not under ceiling

            _movementPerformer.MoveHorizontally(setup.xSpeed, _currentFacingDirectionSign);

            _currentSlideTimer += Time.fixedDeltaTime;
        }

        /// <summary>
        /// Stops slide in progress if any.
        /// </summary>
        public void Stop()
        {
            if (!_performing) return;

            ToggleColliders(true);
            _performing = false;
            _canSlideAt = Time.fixedTime + setup.delay;

            _movementPerformer.StopMovement();

            if (!setup.active) return;

            SlideUpdate.Invoke(_performing);
        }

        protected virtual void ToggleColliders(bool enable)
        {
            if (_collidersToDisable == null) return;

            foreach (Collider2D collider in _collidersToDisable)
            {
                collider.enabled = enable;
            }
        }

        protected virtual bool IsUnderCeiling()
        {
            bool hittingLeft = false;
            bool hittingRight = false;

            Vector2 center = _slidingCollider.bounds.center;

            Vector2 leftPos = center + new Vector2(-_slidingCollider.bounds.extents.x, _slidingCollider.bounds.extents.y);
            Vector2 rightPos = center + new Vector2(_slidingCollider.bounds.extents.x, _slidingCollider.bounds.extents.y);

            RaycastHit2D leftHit = Physics2D.Raycast(leftPos, Vector2.up, _ceilingDetectionLength, _whatIsCeiling);
            RaycastHit2D rightHit = Physics2D.Raycast(rightPos, Vector2.up, _ceilingDetectionLength, _whatIsCeiling);

            if (leftHit.collider != null)
            {
                float leftAngle = Mathf.Round(Vector2.Angle(leftHit.normal, Vector2.down));
                hittingLeft = leftAngle == 0;
            }

            if (rightHit.collider != null)
            {
                float rightAngle = Mathf.Round(Vector2.Angle(rightHit.normal, Vector2.down));
                hittingRight = rightAngle == 0;
            }

            DebugCeilingHit(leftPos, leftHit);
            DebugCeilingHit(rightPos, rightHit);

            return hittingLeft || hittingRight;
        }

        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugCeilingHit(Vector2 pos, RaycastHit2D hit)
        {
            if (!_debugOn) return;
            Debug.DrawRay(pos, Vector2.up * _ceilingDetectionLength, hit.collider ? Color.red : Color.green);
        }

        protected IEnumerator StopAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            Stop();
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Call this in order to Lock slide and
        /// prevent new slides to occur based on
        /// shouldLock boolean.
        /// </summary>
        /// <param name="shouldLock"></param>
        public void Lock(bool shouldLock)
        {
            _slideLocked = shouldLock;
        }

        /// <summary>
        /// Call this to update grouding
        /// </summary>
        /// <param name="newGrounding"></param>
        public void UpdateGronding(bool newGrounding)
        {
            _grounded = newGrounding;
        }

        /// <summary>
        /// Call this to update facing direction sign. -1 for left, 1 for right.
        /// </summary>
        /// <param name="newFacingDirectionSign"></param>
        public void UpdateFacingDirectionSign(float newFacingDirectionSign)
        {
            _currentFacingDirectionSign = newFacingDirectionSign;
        }

        #endregion

        #region Update Seeking

        /// <summary>
        /// Find important components
        /// </summary>
        protected virtual void FindComponents()
        {
            SeekComponent<IPlatformerSlideHandler>(_seekSlideHandler, ref _slideHandler);
            SeekComponent<IPlatformerMovementPerformer>(_seekMovementPerformer, ref _movementPerformer);
            SeekComponent<IPlaftormerGroundingProvider>(_seekGroundingProvider, ref _groundingProvider);
            SeekComponent<IPlatformerFacingDirectionProvider>(_seekHorizontalFacingDirectionProvider, ref _horizontalDirectionProvider);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            _groundingProvider?.GroundingUpdate.AddListener(UpdateGronding);
            _horizontalDirectionProvider?.FacingDirectionSignUpdate.AddListener(UpdateFacingDirectionSign);
            _slideHandler?.SlideRequest.AddListener(Request);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            _groundingProvider?.GroundingUpdate.RemoveListener(UpdateGronding);
            _horizontalDirectionProvider?.FacingDirectionSignUpdate.RemoveListener(UpdateFacingDirectionSign);
            _slideHandler?.SlideRequest.RemoveListener(Request);
        }

        #endregion
    }
}
