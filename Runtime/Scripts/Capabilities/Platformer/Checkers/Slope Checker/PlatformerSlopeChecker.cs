using H2DT.Enums;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using H2DT.Debugging;

namespace H2DT.Capabilities.Platforming
{
    // [AddComponentMenu("Handy 2D Tools/Character Controller/Checkers/SlopeChecker")]
    [RequireComponent(typeof(Collider2D))]
    [DefaultExecutionOrder(-400)]
    public class PlatformerSlopeChecker : Checker, IPlatformerSlopeDataProvider
    {
        #region Inspector

        [Header("Slope Check Collider")]
        [Tooltip("This is optional. You can either specify the collider or leave to this component to find a CapsuleCollider2D. Usefull if you have multiple colliders")]
        [SerializeField]
        protected Collider2D slopeCollider;

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool debugOn = false;

        [Tooltip("This is only informative. Shoul not be touched")]
        [ShowIf("debugOn"), SerializeField, ReadOnly]
        protected bool onSlope = false;

        [ShowIf("debugOn"), SerializeField, ReadOnly]
        protected PlatformerSlopeData currentSlopeData;

        [Header("Detection")]
        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField, Range(1f, 20f)] protected float detectionLength = 2f;

        [Tooltip("Inform what layers should be considered ground"), InfoBox("Without this the component won't work", EInfoBoxType.Warning)]
        [SerializeField, Space] protected LayerMask whatIsGround;

        [Header("Angles")]
        [SerializeField][Range(1, 45)] protected int maxAngle = 45;

        [Header("Detection Positioning")]

        [Tooltip("An offset position for where detection should start on X axis")]
        [SerializeField, Range(-100f, 100f)] protected float positionXOffset = 0f;

        [Tooltip("An offset position for where detection should start on Y axis")]
        [SerializeField, Range(-10f, 100f)] protected float positionYOffset = 0f;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerHorizontalDirectionProvider you can mark this and it will subscribe to its events. PCActions implements it.")]
        [SerializeField] protected bool seekFacingDirectionProvider = false;

        [Foldout("Seekers")]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerGroundingProvider you can mark this and it will subscribe to its events. GroundingChecker2D implements it.")]
        [SerializeField] protected bool seekGroundingProvider = false;

        [Foldout("Available Events:")]
        [Space]
        [Tooltip("Associate this event with callbacks that seek for SlopeData"), InfoBox("You can use these to directly set listeners about this GameObject being on a slope")]
        public UnityEvent<PlatformerSlopeData> SlopeDataUpdateEvent;

        #endregion

        #region Components

        protected IPlaftormerGroundingProvider groundingProvider; // Receives info about GameObject's being grounded.
        protected IPlatformerFacingDirectionProvider facingDirectionProvider; // Receives info about GameObject's movement direction.

        #endregion

        #region Properties

        protected bool grounded = false;
        protected float RealMaxAngle => maxAngle - 0.1f;
        protected HorizontalDirection facingDirection = HorizontalDirection.Right;
        protected int lastAbsoluteDirection = 1;

        #endregion

        #region Getters

        // Direct checking
        public bool OnSlope => onSlope;
        public PlatformerSlopeData Data => currentSlopeData;
        protected int FacingDirectionSign => facingDirection == HorizontalDirection.Right ? 1 : -1;

        // All this convertions are made to make life easier on inspector
        protected float ConvertedDetectionLength => detectionLength / 10;
        protected float PositionXOffset => positionXOffset / 10;
        protected float PositionYOffset => positionYOffset / 10;

        // Event getters
        public UnityEvent<PlatformerSlopeData> SlopeDataUpdate => SlopeDataUpdateEvent;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (slopeCollider == null) slopeCollider = GetComponent<Collider2D>();

            if (whatIsGround == 0)
                Log.Danger($"No ground layer defined for {GetType().Name} on {gameObject.name}");
        }

        protected virtual void Start()
        {
            SubscribeSeekers();
        }

        protected virtual void FixedUpdate()
        {
            SlopeCheck();
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
        /// Checks if is hitting a wall
        /// </summary>
        /// <returns> true if hitting a wall </returns>
        protected virtual void SlopeCheck()
        {
            PlatformerSlopeData data = new PlatformerSlopeData();

            float length = ConvertedDetectionLength; // Just "length" is easier to read.

            CastPositions positions = CalculatePositions(slopeCollider.bounds.center, slopeCollider.bounds.extents, FacingDirectionSign);

            RaycastHit2D centerHit = Physics2D.Raycast(positions.center, Vector2.down, length, whatIsGround);
            RaycastHit2D frontHit = Physics2D.Raycast(positions.front, Vector2.down, length, whatIsGround);
            RaycastHit2D backHit = Physics2D.Raycast(positions.back, Vector2.down, length, whatIsGround);

            data.normalPerpendicular = Vector2.Perpendicular(centerHit.normal).normalized;

            DebugSlopeData(positions, length, centerHit, data);

            // if (!grounded) // can't be on a slope if not grounded
            // {
            //     data.onSlope = false;
            //     UpdateSlopeData(data);
            //     return;
            // }

            if (!centerHit.collider && !frontHit.collider && !backHit.collider) // No hit means not on slope
            {
                data.onSlope = false;
                UpdateSlopeData(data);
                return;
            }

            // Angles must be round up so comparisons won't be broken up.
            float frontAngle = Mathf.Round(Vector2.Angle(frontHit.normal, Vector2.up));
            float centerAngle = Mathf.Round(Vector2.Angle(centerHit.normal, Vector2.up));
            float backAngle = Mathf.Round(Vector2.Angle(backHit.normal, Vector2.up));

            if (Mathf.Abs(centerAngle) == 0 && Mathf.Abs(frontAngle) == 0 && Mathf.Abs(backAngle) == 0) // On Flat ground
            {
                data.onSlope = false;
                UpdateSlopeData(data);
                return;
            }

            if (centerAngle > maxAngle || frontAngle > maxAngle || backAngle > maxAngle) // On slope but too high
            {
                data.onSlope = true;
                data.higherThanMax = true;
                UpdateSlopeData(data);
                return;
            }

            data.onSlope = true;
            data.ascending = frontHit.point.y > backHit.point.y;
            data.descending = centerHit.point.y < backHit.point.y;
            data.exitingFromAbove = Mathf.Abs(frontAngle) == 0 && data.ascending;
            data.exitingFromBelow = Mathf.Abs(centerAngle) == 0 && data.descending;

            UpdateSlopeData(data);
        }

        /// <summary>
        /// Calculates positions where to cast from based on collider properties and facing direction.
        /// </summary>
        /// <param name="colliderCenter"></param>
        /// <param name="colliderExtents"></param>
        /// <param name="dirX"></param>
        /// <returns></returns>
        protected virtual CastPositions CalculatePositions(Vector2 colliderCenter, Vector2 colliderExtents, int dirX)
        {
            Vector2 centerPos;
            Vector2 frontPos;
            Vector2 backPos;

            centerPos = colliderCenter + new Vector2(0, -colliderExtents.y + PositionYOffset);

            if (dirX > 0)
            {
                frontPos = colliderCenter + new Vector2(colliderExtents.x + PositionXOffset, -colliderExtents.y + PositionYOffset);
                backPos = colliderCenter + new Vector2(-colliderExtents.x - PositionXOffset, -colliderExtents.y + PositionYOffset);
            }
            else
            {
                frontPos = colliderCenter + new Vector2(-colliderExtents.x - PositionXOffset, -colliderExtents.y + PositionYOffset);
                backPos = colliderCenter + new Vector2(colliderExtents.x + PositionXOffset, -colliderExtents.y + PositionYOffset);
            }

            return new CastPositions(frontPos, centerPos, backPos);
        }

        protected virtual void UpdateSlopeData(PlatformerSlopeData newData)
        {
            currentSlopeData = newData;
            onSlope = newData.onSlope;
            SlopeDataUpdate.Invoke(newData);
        }

        #region Callbacks

        public virtual void UpdateFacingDirection(HorizontalDirection newFacingDirection)
        {
            facingDirection = newFacingDirection;
        }

        public virtual void UpdateGrounding(bool newGrounding)
        {
            grounded = newGrounding;
        }

        #endregion        

        #region Update Seeking

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            UnsubscribeSeekers();
            if (seekFacingDirectionProvider)
            {
                facingDirectionProvider = GetComponent<IPlatformerFacingDirectionProvider>();
                if (facingDirectionProvider == null)
                    Debug.LogWarning("Component SlopeChecker2D might not work properly. It is marked to seek for an IMovementDirectionUpdater but it could not find any.");
                facingDirectionProvider?.FacingDirectionUpdate.AddListener(UpdateFacingDirection);
            }

            if (seekGroundingProvider)
            {
                groundingProvider = GetComponent<IPlaftormerGroundingProvider>();
                if (groundingProvider == null)
                    Debug.LogWarning("Component SlopeChecker2D might not work properly. It is marked to seek for an IPlatformerGroundingProvider but it could not find any.");
                groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
            }
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
            facingDirectionProvider?.FacingDirectionUpdate.RemoveListener(UpdateFacingDirection);
        }

        #endregion

        /// <summary>
        /// Represents positions where to RayCast from
        /// </summary>
        protected struct CastPositions
        {
            public Vector2 front;
            public Vector2 center;
            public Vector2 back;

            public CastPositions(Vector2 frontPos, Vector2 centerPos, Vector2 backPos)
            {
                front = frontPos;
                center = centerPos;
                back = backPos;
            }
        }

        protected virtual void DebugSlopeData(CastPositions positions, float length, RaycastHit2D centerHit, PlatformerSlopeData data)
        {
            if (!debugOn) return;

            Debug.DrawRay(positions.center, Vector2.down * length, centerHit ? Color.red : Color.green);
            Debug.DrawRay(positions.front, Vector2.down * length, Color.yellow);
            Debug.DrawRay(positions.back, Vector2.down * length, Color.cyan);
            Debug.DrawRay(centerHit.point, data.normalPerpendicular, Color.blue);
            Debug.DrawRay(centerHit.point, centerHit.normal, Color.black);
        }

        // Debug:
        // Debug.DrawRay(objCollider.bounds.center, Vector2.down* (objCollider.bounds.extents.y + length), centerHit? Color.red : Color.green);
        // Debug.DrawRay(centerHit.point, data.normalPerpendicular, Color.blue);
        // Debug.DrawRay(centerHit.point, centerHit.normal, Color.black);
        // Debug.DrawRay(positions.front, Vector2.down* length * 2, Color.yellow);
        // Debug.DrawRay(positions.back, Vector2.down* length * 2, Color.cyan);
    }
}
