using UnityEngine;
using UnityEngine.Events;
using H2DT.NaughtyAttributes;
using H2DT.Capabilities;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Movement/PlatformerWallGrab")]
    public class PlatformerWallGrab : LearnableAbilityComponent<PlatformerWallGrabSetup>, IPlatformerWallGrabPerformer
    {

        #region Editor

        [Header("Dependencies")]
        [InfoBox("If you prefer you can read the docs on how to feed this component directly through one of your scripts.")]
        [Space]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerMovementPerformer you can mark this and it will subscribe to its events. DynamicPlatformerMovement, for example, implements it.")]
        [SerializeField]
        protected bool _seekMovementPerformer = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerWallHitDataProvider you can mark this and it will subscribe to its events. WallHitChecker, for example, implements it.")]
        [SerializeField]
        protected bool _seekWallHitDataProvider = false;

        [SerializeField]
        [Space]
        protected bool _debugOn;

        [ShowIf("_debugOn")]
        [ReadOnly]
        [SerializeField]
        protected bool _performing = false;

        #endregion

        #region Interfaces

        protected IPlatformerMovementPerformer _movementPerformer;
        protected IPlatformerWallHitDataProvider _wallHitDataProvider;

        #endregion

        #region Fields

        protected float _gravityBeforeRequest;
        protected WallHitData _wallHitData;

        #endregion

        #region Components

        #endregion

        #region Properties

        public IPlatformerMovementPerformer movementPerformer { get { return _movementPerformer; } set { _movementPerformer = value; } }
        public IPlatformerWallHitDataProvider WallHitDataProvider { get { return _wallHitDataProvider; } set { _wallHitDataProvider = value; } }

        #endregion

        #region Getters

        public bool active => setup.active;
        public bool performing => _performing;

        // Events
        public UnityEvent<bool> WallGrabUpdate => setup.WallGrabUpdate;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            FindComponents();
        }

        protected virtual void OnEnable()
        {
            SubscribeSeekers();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region  Logic

        /// <summary>
        /// Evaluates if can start the wall grab process. If so Perform() should be called each physics update.
        /// </summary>
        public void Request(float movementDirectionSign)
        {
            if (!setup.active || _performing) return;

            if ((movementDirectionSign < 0 && _wallHitData.leftHitting) || (movementDirectionSign > 0 && _wallHitData.rightHitting))
            {
                if (setup.changeGravityScale)
                {
                    _gravityBeforeRequest = _movementPerformer.currentGravityScale;
                    _movementPerformer.ChangeGravityScale(setup.gravityScale);
                }

                _performing = true;
                WallGrabUpdate.Invoke(_performing);
            }
        }

        /// <summary>
        /// Should be called on Fixed (Physics) Update. If direction passed as 0 means no movement.
        /// Note that gravity can still affect the RigidBody2D
        /// </summary>
        /// <param name="directionSign">The direction sign to move. -1 down, 1 up</param>
        public void Perform(float directionSign = 0)
        {
            if (!_performing) return;

            if (setup.canMove && directionSign != 0)
            {
                _movementPerformer.MoveVertically(setup.moveSpeed, directionSign);
            }
            else if (setup.naturalSlide)
            {
                _movementPerformer.MoveVertically(setup.naturalSlideSpeed, -1);
            }
            else
            {
                _movementPerformer.StopMovement();
            }
        }

        /// <summary>
        /// Stops Wall slide progress.
        /// </summary>
        public void Stop()
        {
            if (!_performing) return;

            _performing = false;

            if (setup.changeGravityScale)
            {
                _movementPerformer.ChangeGravityScale(_gravityBeforeRequest);
            }

            if (!setup.active) return;

            WallGrabUpdate.Invoke(_performing);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Call this to update wall hit data.
        /// </summary>
        /// <param name="newWallGrab"></param>
        public void UpdateWallHitData(WallHitData newWallHitData)
        {
            _wallHitData = newWallHitData;
        }

        #endregion          

        #region Update Seeking

        protected virtual void FindComponents()
        {
            SeekComponent<IPlatformerMovementPerformer>(_seekMovementPerformer, ref _movementPerformer);
            SeekComponent<IPlatformerWallHitDataProvider>(_seekWallHitDataProvider, ref _wallHitDataProvider);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            _wallHitDataProvider?.WallHitDataUpdate.AddListener(UpdateWallHitData);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            _wallHitDataProvider?.WallHitDataUpdate.RemoveListener(UpdateWallHitData);
        }

        #endregion
    }
}
