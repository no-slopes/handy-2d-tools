using H2DT.Capabilities;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    [CreateAssetMenu(fileName = "New PlatformerJumpSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Jump")]
    public class PlatformerJumpSetup : Learnable
    {
        #region Inspector

        [Header("Jump Setup")]
        [Space]
        [SerializeField]
        protected SimpleSpriteAnimation _ascendingAnimation;

        [Space]
        [SerializeField]
        protected SimpleSpriteAnimation _fallingAnimation;

        [Tooltip("The amount of force wich will be proportionaly applyed to Y axis.")]
        [Range(1f, 20f)]
        [Space]
        [SerializeField]
        protected float _force = 5f;

        [Label("Duration")]
        [Tooltip("Period of time in seconds during which force will be applyed positively to Y axis.")]
        [Range(0.1f, 2f)]
        [SerializeField]
        protected float _duration = 0.35f;

        [Range(1f, 50f)]
        [SerializeField]
        private float _maxAbsoluteAscensionSpeed = 25;

        [Header("Fall")]
        [Space]
        [Range(1f, 20f)]
        [SerializeField]
        private float _fallGravityScale = 10f;

        [Range(1f, 50f)]
        [SerializeField]
        private float _maxAbsoluteFallSpeed = 30;

        [Header("Coyote Time")]
        [Tooltip("Mark this if you want coyote time to be applyed.")]
        [Space]
        [SerializeField]
        protected bool _hasCoyoteTime = false;

        [Label("Coyote Time Rate")]
        [Tooltip("Used to calculate for how long character can still jump in case of not being grounded anymore.")]
        [ShowIf("hasCoyoteTime")]
        [SerializeField]
        protected float _coyoteTime = 0.15f;

        [Header("Jump Buffer")]
        [Label("Jump Buffer Rate")]
        [Tooltip("Used to allow character jumping even though a jump request has been made before it is considered grounded.")]
        [Space]
        [SerializeField]
        protected float _jumpBufferTime = 0.15f;

        [Header("Wall jumps")]
        [Label("Can Wall Jump")]
        [Tooltip("Mark this if character can jump from a wall.")]
        [Space]
        [SerializeField]
        protected bool _canWallJump = false;

        [Tooltip("Mark this if you want coyote time to be applyed for wall jumps.")]
        [SerializeField]
        [ShowIf("_canWallJump")]
        [Space]
        protected bool _hasWallCoyoteTime = false;

        [Label("Coyote Time Rate")]
        [Tooltip("Used to calculate for how long character can still jump in case of not on wall anymore.")]
        [ShowIf("HasWallCoyoteTime")]
        [SerializeField]
        [Space]
        protected float _wallCoyoteTime = 0.15f;

        [ShowIf("_canWallJump")]
        [Space]
        [SerializeField]
        protected bool _applyForceAwayFromWall = false;

        [ShowIf("AppliesForceAwayFromWall")]
        [SerializeField]
        [Space]
        protected float _forceAwayFromWall = 100f;

        [ShowIf("AppliesForceAwayFromWall")]
        [SerializeField]
        protected float _forceAwayFromWallDuration = 0.2f;

        [Header("Extra Jumps")]
        [Label("Has Extra Jumps")]
        [Space]
        [SerializeField]
        protected bool _hasExtraJumps = false;

        [Tooltip("The amount of extra jumps the character can acumulate to perform sequentially after main jump")]
        [ShowIf("hasExtraJumps")]
        [Space]
        [SerializeField]
        protected int _extraJumps = 1;

        [Tooltip("The amount of force wich will be proportionaly applyed to Y axis.")]
        [ShowIf("hasExtraJumps")]
        [SerializeField]
        protected float _extraJumpForce = 100f;

        [Tooltip("Period of time in seconds during which force will be applyed positively to Y axis.")]
        [ShowIf("hasExtraJumps")]
        [SerializeField]
        protected float _extraJumpDuration = 0.35f;

        [Foldout("Jump Events")]
        [Label("Jump Update")]
        [Space]
        [SerializeField]
        public UnityEvent<bool> JumpUpdate;

        [Foldout("Jump Events")]
        [Label("Extra Jump Update")]
        [SerializeField]
        public UnityEvent<bool> ExtraJumpUpdate;

        #endregion

        #region Properties

        protected bool HasWallCoyoteTime => _canWallJump && _hasWallCoyoteTime;
        protected bool AppliesForceAwayFromWall => _canWallJump && _applyForceAwayFromWall;

        #endregion

        #region Getters

        public SimpleSpriteAnimation AscendingAnimation => _ascendingAnimation;
        public SimpleSpriteAnimation FallingAnimation => _fallingAnimation;

        // Ascension
        public float force { get { return _force; } set { _force = value; } }
        public float duration { get { return _duration; } set { _duration = value; } }
        public float maxAbsoluteAscensionSpeed => Mathf.Abs(_maxAbsoluteAscensionSpeed);


        // Fall
        public float fallGravityScale => _fallGravityScale;
        public float maxAbsoluteFallSpeed => Mathf.Abs(_maxAbsoluteFallSpeed);

        // Coyote time
        public bool hasCoyoteTime { get { return _hasCoyoteTime; } set { _hasCoyoteTime = value; } }
        public float coyoteTime { get { return _coyoteTime; } set { _coyoteTime = value; } }

        // Jump buffer
        public float jumpBufferTime { get { return _jumpBufferTime; } set { _jumpBufferTime = value; } }

        // Wall jumps
        public bool canWallJump { get { return _canWallJump; } set { _canWallJump = value; } }
        public bool hasWallCoyoteTime { get { return _hasWallCoyoteTime; } set { _hasWallCoyoteTime = value; } }
        public float wallCoyoteTime { get { return _wallCoyoteTime; } set { _wallCoyoteTime = value; } }
        public bool applyForceAwayFromWall { get { return _applyForceAwayFromWall; } set { _applyForceAwayFromWall = value; } }
        public float forceAwayFromWall { get { return _forceAwayFromWall; } set { _forceAwayFromWall = value; } }
        public float forceAwayFromWallDuration { get { return _forceAwayFromWallDuration; } set { _forceAwayFromWallDuration = value; } }

        // Extra jumps
        public bool hasExtraJumps { get { return _hasExtraJumps; } set { _hasExtraJumps = value; } }

        public int extraJumps
        {
            get { return _extraJumps; }
            set
            {
                if (value < 0)
                {
                    _extraJumps = 0;
                    return;
                }

                _extraJumps = value;
            }
        }

        public float extraJumpForce { get { return _extraJumpForce; } set { _extraJumpForce = value; } }
        public float extraJumpDuration { get { return _extraJumpDuration; } set { _extraJumpDuration = value; } }

        #endregion

        #region Logic

        public virtual void ActivateExtraJumps(bool active)
        {
            _hasExtraJumps = active;
        }

        public virtual void SetWallJump(bool shouldWallJump)
        {
            _canWallJump = shouldWallJump;
        }

        #endregion
    }
}
