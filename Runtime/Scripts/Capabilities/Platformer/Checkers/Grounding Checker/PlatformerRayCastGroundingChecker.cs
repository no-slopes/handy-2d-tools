using H2DT.Capabilities.Generic;
using H2DT.Debugging;
using H2DT.Enums;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Checkers/PlatformerRayCastGroundingChecker")]
    [RequireComponent(typeof(Collider2D))]
    public class PlatformerRayCastGroundingChecker : Checker, IPlaftormerGroundingProvider, IPlatformerGroundingChecker
    {
        #region Inspector 

        [Header("Flip information")]
        [Tooltip("If this component should consider vertical flipping")]
        [SerializeField]
        private bool _acknowledgeVerticalFlip = false;

        [SerializeField]
        private Flip2D _flip;

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool _debugOn = false;

        [Tooltip("This is only informative. Shoul not be touched")]
        [ShowIf("_debugOn")]
        [SerializeField]
        [ReadOnly]
        protected bool _grounded = false;


        [Header("Ground check Collider")]
        [Tooltip("This is optional. You can either specify the collider or leave to this component to find a CapsuleCollider2D. Usefull if you have multiple colliders")]
        [SerializeField]
        protected Collider2D _groundingCollider;

        [Header("Layers")]
        [InfoBox("Without this the component won't work", EInfoBoxType.Warning)]
        [Tooltip("Inform what layers should be considered ground")]
        [SerializeField]
        [Space]
        protected LayerMask _whatIsGround;

        [Header("Directions")]
        [Tooltip("The checking direction")]
        [SerializeField]
        protected VerticalDirection _verticalDirection = VerticalDirection.Down;

        // Right stuff
        [Header("Right Detection")]
        [Tooltip("If right check should be enabled")]
        [SerializeField]
        protected bool _checkRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _rightDetectionLength = 2f;

        [Tooltip("An offset position for where right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _rightPositionXOffset = 0f;

        [Tooltip("An offset position for where rightdetection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _rightPositionYOffset = 0f;

        // Left Stuff
        [Header("Left Detection")]
        [Tooltip("If left check should be enabled")]
        [SerializeField]
        protected bool _checkLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _leftDetectionLength = 2f;

        [Tooltip("An offset position for where left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _leftPositionXOffset = 0f;

        [Tooltip("An offset position for where left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _leftPositionYOffset = 0f;

        // Center Stuff
        [Header("Center Detection")]
        [Tooltip("If center check should be enabled")]
        [SerializeField] protected bool _checkCenter = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _centerDetectionLength = 2f;

        [Tooltip("An offset position for where center detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 1000f)]
        protected float _centerPositionYOffset = 0f;

        [Foldout("Available Events:")]
        [Space]
        [InfoBox("You can use these to directly set listeners about this GameObject's grounding")]
        [SerializeField]
        protected UnityEvent<bool> _groundingUpdate;

        #endregion

        #region Interfaces

        #endregion

        #region Components

        protected Rigidbody2D _rb;

        #endregion

        #region fields

        protected float lengthConvertionRate = 100f;
        protected float positionOffsetConvertionRate = 100f;

        #endregion

        #region Properties

        public LayerMask whatIsGround { get { return _whatIsGround; } set { _whatIsGround = value; } }

        // All this convertions are made to make life easier on inspector
        protected float rightLengthConverted => _rightDetectionLength / lengthConvertionRate;
        protected float leftLengthConverted => _leftDetectionLength / lengthConvertionRate;
        protected float centerLengthConverted => _centerDetectionLength / lengthConvertionRate;

        // Positioning offset convertions
        protected float rightPositionXOffset => _rightPositionXOffset / positionOffsetConvertionRate;
        protected float rightPositionYOffset => _rightPositionYOffset / positionOffsetConvertionRate;
        protected float centerPositionYOffset => _centerPositionYOffset / positionOffsetConvertionRate;
        protected float leftPositionXOffset => _leftPositionXOffset / positionOffsetConvertionRate;
        protected float leftPositionYOffset => _leftPositionYOffset / positionOffsetConvertionRate;

        #endregion

        #region Getters

        /// <summary>
        /// The whole purpose of this component. Behold: The ground check.
        /// </summary>
        /// <returns> true if... grounded! </returns>
        public bool grounded => _grounded;

        public UnityEvent<bool> GroundingUpdate => _groundingUpdate;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponents();

            if (_groundingCollider == null) _groundingCollider = GetComponent<Collider2D>();
        }

        protected virtual void FixedUpdate()
        {
            EvaluateGroundingConsideringVerticalDirection();
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

        /// <summary>
        /// Casts rays to determine if character is grounded.
        /// </summary>
        /// <returns> true if grounded </returns>
        public virtual bool EvaluateGroundingConsideringVerticalDirection()
        {
            // If going going to oposite direction of current vertical direction, consider not grounded
            if (_rb != null && (_verticalDirection == VerticalDirection.Down && _rb.velocity.y > 0) || (_verticalDirection == VerticalDirection.Up && _rb.velocity.y < 0))
            {
                UpdateGroundedStatus(false); // Update grounded property and fire events 
                return false;
            }

            return EvaluateGrounding();
        }

        /// <summary>
        /// Casts rays to determine if character is grounded.
        /// </summary>
        /// <returns> true if grounded </returns>
        public virtual bool EvaluateGrounding()
        {

            CastPositions positions = CalculatePositions(_groundingCollider.bounds.center, _groundingCollider.bounds.extents);

            Vector2 castDirection = _verticalDirection == VerticalDirection.Down ? Vector2.down : Vector2.up;

            RaycastHit2D rightHit = Physics2D.Raycast(positions.right, castDirection, rightLengthConverted, _whatIsGround);
            RaycastHit2D leftHit = Physics2D.Raycast(positions.left, castDirection, leftLengthConverted, _whatIsGround);
            RaycastHit2D centerHit = Physics2D.Raycast(positions.center, castDirection, centerLengthConverted, _whatIsGround);

            bool check = (_checkRight && rightHit.collider != null) || (_checkCenter && centerHit.collider != null) || (_checkLeft && leftHit.collider != null);

            UpdateGroundedStatus(check); // Update grounded property and fire events
            DebugGroundCheck(castDirection, positions, rightHit, leftHit, centerHit);

            return check;
        }

        /// <summary>
        /// Updates grounded status based on GroundingUpdate parameter.
        /// This will send an UnityEvent<bool> case grounding status 
        /// has changed.
        /// </summary>
        /// <param name="groundingUpdate"></param>
        protected virtual void UpdateGroundedStatus(bool groundingUpdate)
        {
            if (_grounded == groundingUpdate) return;
            _grounded = groundingUpdate;
            _groundingUpdate.Invoke(_grounded);
        }

        /// <summary>
        /// Calculates positions where to cast from based on collider properties.
        /// </summary>
        /// <param name="center"> Stands for the collider center as a Vector2 </param>
        /// <param name="extents"> Stands for the collider extents as a Vector 2 </param>
        /// <returns></returns>
        protected CastPositions CalculatePositions(Vector2 center, Vector2 extents)
        {
            if (_verticalDirection == VerticalDirection.Down)
            {
                Vector2 rightPos = center + new Vector2(extents.x + rightPositionXOffset, -extents.y + rightPositionYOffset);
                Vector2 leftPos = center + new Vector2(-extents.x - leftPositionXOffset, -extents.y + leftPositionYOffset);
                Vector2 centerPos = center + new Vector2(0, -extents.y + centerPositionYOffset);

                return new CastPositions(rightPos, centerPos, leftPos);
            }
            else
            {
                Vector2 rightPos = center + new Vector2(extents.x + rightPositionXOffset, extents.y + rightPositionYOffset);
                Vector2 leftPos = center + new Vector2(-extents.x - leftPositionXOffset, extents.y + leftPositionYOffset);
                Vector2 centerPos = center + new Vector2(0, extents.y + centerPositionYOffset);

                return new CastPositions(rightPos, centerPos, leftPos);
            }
        }


        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugGroundCheck(Vector2 castDirection, CastPositions positions, RaycastHit2D rightHit, RaycastHit2D leftHit, RaycastHit2D centerHit)
        {
            if (!_debugOn) return;

            if (_checkRight)
                Debug.DrawRay(positions.right, castDirection * rightLengthConverted, rightHit.collider ? Color.red : Color.green);

            if (_checkLeft)
                Debug.DrawRay(positions.left, castDirection * leftLengthConverted, leftHit.collider ? Color.red : Color.green);

            if (_checkCenter)
                Debug.DrawRay(positions.center, castDirection * centerLengthConverted, centerHit.collider ? Color.red : Color.green);
        }

        /// <summary>
        /// Represents positions where to RayCast from
        /// </summary>
        protected struct CastPositions
        {
            public Vector2 right;
            public Vector2 center;
            public Vector2 left;

            public CastPositions(Vector2 rightPos, Vector2 centerPos, Vector2 leftPos)
            {
                right = rightPos;
                center = centerPos;
                left = leftPos;
            }
        }

        protected void UpdateVerticalDirection(VerticalDirection newVerticaldirection)
        {
            _verticalDirection = newVerticaldirection;
        }

        #region Update Seeking

        protected virtual void FindComponents()
        {
            FindComponent<Rigidbody2D>(ref _rb);

            if (_acknowledgeVerticalFlip && _flip == null)
                _flip = GetComponent<Flip2D>();
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            if (_acknowledgeVerticalFlip)
            {
                if (_flip == null)
                {
                    Log.Warning($"{name} - {GetType().Name} - Shoul acknowledge vertical flip but 2DFlip component not found.");
                }
                _flip?.VerticalDirectionUpdate.AddListener(UpdateVerticalDirection);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            _flip?.VerticalDirectionUpdate.RemoveListener(UpdateVerticalDirection);
        }

        #endregion
    }
}
