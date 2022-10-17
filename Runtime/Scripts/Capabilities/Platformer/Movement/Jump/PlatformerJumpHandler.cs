using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using H2DT.NaughtyAttributes;
using H2DT.Capabilities;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Movement/PlatformerJumpHandler")]
    public class PlatformerJumpHandler : LearnableAbilityComponent<PlatformerJumpSetup>
    {
        #region  Fields

        private IPlaftormerGroundingProvider _groundingProvider;
        private IPlatformerWallHitDataProvider _wallHitDataProvider;

        private bool _grounded;
        private WallHitData _wallHitData;

        private float _jumpRequestedAt;
        private bool _jumpRequestPersists = false;
        private int _extraJumpsLeft;
        private float _canResetJumpAt;
        private float _jumpResetRequestedAt;
        private float _coyoteTimeCounter;
        private float _wallCoyoteTimeCounter;
        private float _jumpResetGhostingTime;
        protected bool _jumpLocked;

        #endregion

        #region Getters

        public float jumpRequestedAt { get => _jumpRequestedAt; set => _jumpRequestedAt = value; }
        public bool jumpRequestPersists { get => _jumpRequestPersists; set => _jumpRequestPersists = value; }
        public int extraJumpsLeft { get => _extraJumpsLeft; set => _extraJumpsLeft = value; }
        public float canResetJumpAt { get => _canResetJumpAt; set => _canResetJumpAt = value; }
        public float jumpResetRequestedAt { get => _jumpResetRequestedAt; set => _jumpResetRequestedAt = value; }
        public float coyoteTimeCounter { get => _coyoteTimeCounter; set => _coyoteTimeCounter = value; }
        public float wallCoyoteTimeCounter { get => _wallCoyoteTimeCounter; set => _wallCoyoteTimeCounter = value; }
        public float jumpResetGhostingTime { get => _jumpResetGhostingTime; set => _jumpResetGhostingTime = value; }

        public bool canStartJump => !_jumpLocked && setup.active && (canStartJumpFromGround || canStartJumpFromWall);
        public bool canStartExtraJump => setup.active && setup.hasExtraJumps && _extraJumpsLeft > 0;

        public bool canStartJumpFromGround => !_jumpLocked && hasCoyoteTime ? _coyoteTimeCounter > 0f : _grounded;
        public bool hasCoyoteTime => setup.hasCoyoteTime && setup.coyoteTime != 0;

        public bool canStartJumpFromWall => setup.canWallJump && wallCoyoteCheck;
        public bool wallCoyoteCheck => hasWallCoyoteTime ? _wallCoyoteTimeCounter > 0f : OnWall; // The wallCoyoteTimeCounter considers if grabbing wall
        public bool hasWallCoyoteTime => setup.hasWallCoyoteTime && setup.wallCoyoteTime != 0;
        public bool OnWall => !_grounded && (_wallHitData.leftHitting || _wallHitData.rightHitting);

        public bool inJumpBufferTimeWindow => Time.time < _jumpRequestedAt + setup.jumpBufferTime;

        #endregion

        #region  Mono

        protected override void Awake()
        {
            base.Awake();
            FindComponents();
        }

        protected void OnEnable()
        {
            SubscribeActions();
        }

        protected void OnDisable()
        {
            UnsubscribeActions();
        }

        protected void Update()
        {
            HandleCoyotes();
        }

        #endregion

        #region Logic        

        public void LockJump(bool shouldLock)
        {
            _jumpLocked = shouldLock;
        }

        protected void HandleCoyotes()
        {
            if (hasCoyoteTime && _grounded)
            {
                coyoteTimeCounter = setup.coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (hasWallCoyoteTime && OnWall)
            {
                wallCoyoteTimeCounter = setup.wallCoyoteTime;
            }
            else
            {
                wallCoyoteTimeCounter -= Time.deltaTime;
            }
        }

        #endregion

        #region Updates

        protected void UpdateGrounding(bool newGrounding)
        {
            _grounded = newGrounding;
        }

        protected void UpdateWallHitData(WallHitData newData)
        {
            _wallHitData = newData;
        }

        #endregion

        #region Components

        protected void FindComponents()
        {
            FindComponent<IPlaftormerGroundingProvider>(ref _groundingProvider);
            FindComponent<IPlatformerWallHitDataProvider>(ref _wallHitDataProvider);
        }

        protected void SubscribeActions()
        {
            _groundingProvider.GroundingUpdate.AddListener(UpdateGrounding);
            _wallHitDataProvider.WallHitDataUpdate.AddListener(UpdateWallHitData);
        }

        protected void UnsubscribeActions()
        {
            _groundingProvider.GroundingUpdate.RemoveListener(UpdateGrounding);
            _wallHitDataProvider.WallHitDataUpdate.RemoveListener(UpdateWallHitData);
        }

        #endregion

    }
}
