using H2DT.Capabilities;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{

    [CreateAssetMenu(fileName = "New DynamicPlatformerWallGrabSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Wall Grab")]
    public class PlatformerWallGrabSetup : Learnable
    {
        [Header("Wall Grab Setup")]
        [Tooltip("The Ascending animation")]
        [Space]
        [SerializeField]
        protected SimpleSpriteAnimation _ascendingAnimation;

        [Tooltip("The Idle animation")]
        [SerializeField]
        protected SimpleSpriteAnimation _idleAnimation;

        [Tooltip("The Descending animation")]
        [SerializeField]
        protected SimpleSpriteAnimation _desendingAnimation;

        [Tooltip("The Sliding animation")]
        [SerializeField]
        protected SimpleSpriteAnimation _slidingAnimation;

        [Label("Change gravity on request")]
        [Tooltip("If the gravity scale should be changed on wall grab's request")]
        [SerializeField]
        [Space]
        protected bool _changeGravityScale = true;

        [Tooltip("The gravity scale to be applyed while on a wall")]
        [ShowIf("_changeGravityScale")]
        [SerializeField]
        protected float _gravityScale = 0;

        [Tooltip("If character can move up or down when on a wall")]
        [SerializeField]
        [Space]
        protected bool _canMove = true;

        [Tooltip("The speed apllyed on Y axis while moving on a wall")]
        [SerializeField]
        [ShowIf("canMove")]
        [Range(0, 100f)]
        protected float _moveSpeed = 1;

        [Tooltip("If character should naturally start sliding down when on a wall")]
        [SerializeField]
        [Space]
        protected bool _naturalSlide = false;

        [Tooltip("The slide speed.")]
        [ShowIf("_naturalSlide")]
        [SerializeField]
        [Range(0, 100f)]
        protected float _naturalSlideSpeed = 0;

        [Header("Jump From Walls")]
        [SerializeField]
        protected PlatformerJumpSetup _jumpSetup;

        [SerializeField]
        [ShowIf("hasJumpSetup")]
        protected bool _alsoActivateJumpOnWalls = false;

        [Foldout("Wall Slide Events")]
        [Label("Wall Slide Performed")]
        [SerializeField]
        [Space]
        protected UnityEvent<bool> _wallGrabUpdate;

        #region Getters

        public SimpleSpriteAnimation AscendingAnimation => _ascendingAnimation;
        public SimpleSpriteAnimation IdleAnimation => _idleAnimation;
        public SimpleSpriteAnimation DesendingAnimation => _desendingAnimation;
        public SimpleSpriteAnimation SlidingAnimation => _slidingAnimation;

        public bool changeGravityScale { get { return _changeGravityScale; } set { _changeGravityScale = value; } }
        public float gravityScale { get { return _gravityScale; } set { _gravityScale = value; } }

        public bool canMove { get { return _canMove; } set { _canMove = value; } }
        public float moveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }

        public bool naturalSlide { get { return _naturalSlide; } set { _naturalSlide = value; } }
        public float naturalSlideSpeed { get { return _naturalSlideSpeed; } set { _naturalSlideSpeed = value; } }

        public UnityEvent<bool> WallGrabUpdate => _wallGrabUpdate;

        #endregion

        #region Properties

        protected bool hasJumpSetup => _jumpSetup != null;

        #endregion

        #region Logic

        public override void Activate()
        {
            base.Activate();

            if (_jumpSetup == null) return;

            if (_alsoActivateJumpOnWalls)
            {
                _jumpSetup.SetWallJump(true);
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_jumpSetup == null) return;

            if (_alsoActivateJumpOnWalls)
            {
                _jumpSetup.SetWallJump(false);
            }
        }

        #endregion
    }

}
