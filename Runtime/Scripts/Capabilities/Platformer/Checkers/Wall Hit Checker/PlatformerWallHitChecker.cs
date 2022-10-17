using H2DT.Debugging;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Checkers/PlatformerWallHitChecker")]
    [RequireComponent(typeof(Collider2D))]
    public class PlatformerWallHitChecker : Checker, IPlatformerWallHitDataProvider, IPlatformerWallHitChecker
    {
        #region Inspector

        [Header("Debug")]
        [Tooltip("Turn this on and get some visual feedback. Do not forget to turn your Gizmos On")]
        [SerializeField]
        protected bool _debugOn = false;

        [Tooltip("This is only informative. Shoul not be touched")]
        [ShowIf("_debugOn")]
        [SerializeField]
        [ReadOnly]
        protected bool _hittingWall = false;

        [Header("Ground check Collider")]
        [Tooltip("This is optional. You can either specify the collider or leave to this component to find a CapsuleCollider2D. Usefull if you have multiple colliders")]
        [SerializeField]
        protected Collider2D _wallHitCollider;

        [Header("Layers")]
        [InfoBox("Without this the component won't work", EInfoBoxType.Warning)]
        [Tooltip("Inform what layers should be considered wall")]
        [SerializeField]
        protected LayerMask _whatIsWall;

        [Tooltip("Inform what layers should be considered wall")]
        [SerializeField]
        [Tag]
        protected string _tagToIgnore;

        // Center Stuff
        [Header("Center Left Detection")]
        [Space]
        [Tooltip("If center left check should be enabled")]
        [SerializeField]
        protected bool _checkCenterLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _centerLeftDetectionLength = 2f;

        [Header("Center Right Detection")]
        [Space]
        [Tooltip("If center right check should be enabled")]
        [SerializeField]
        protected bool _checkCenterRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _centerRightDetectionLength = 2f;

        // Right stuff
        [Header("Upper Right Detection")]
        [Space]
        [Tooltip("If upper right check should be enabled")]
        [SerializeField]
        protected bool _checkUpperRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _upperRightDetectionLength = 2f;

        [Tooltip("An offset position for where upper right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _upperRightPositionXOffset = 0f;

        [Tooltip("An offset position for where upper right detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _upperRightPositionYOffset = 0f;

        [Header("Lower Right Detection")]
        [Space]
        [Tooltip("If lower right check should be enabled")]
        [SerializeField]
        protected bool _checkLowerRight = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _lowerRightDetectionLength = 2f;

        [Tooltip("An offset position for where lower right detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _lowerRightPositionXOffset = 0f;

        [Tooltip("An offset position for where lower right detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _lowerRightPositionYOffset = 0f;

        // Left Stuff

        [Header("Upper Left Detection")]
        [Space]
        [Tooltip("If left upper check should be enabled")]
        [SerializeField]
        protected bool _checkUpperLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _upperLeftDetectionLength = 2f;

        [Tooltip("An offset position for where upper left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _upperLeftPositionXOffset = 0f;

        [Tooltip("An offset position for where upper left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _upperLeftPositionYOffset = 0f;

        [Header("Lower Left Detection")]
        [Space]
        [Tooltip("If left lower check should be enabled")]
        [SerializeField]
        protected bool _checkLowerLeft = true;

        [Tooltip("Detection's length. tweak this to suit your needs")]
        [SerializeField]
        [Range(1f, 100f)]
        protected float _lowerLeftDetectionLength = 2f;

        [Tooltip("An offset position for where lower left detection should start on X axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _lowerLeftPositionXOffset = 0f;

        [Tooltip("An offset position for where lower left detection should start on Y axis")]
        [SerializeField]
        [Range(-100f, 100f)]
        protected float _lowerLeftPositionYOffset = 0f;

        [Foldout("Available Events")]
        [Tooltip("Associate this event with callbacks that seek for WallHitData"), InfoBox("You can use these to directly set listeners about this GameObject colliding with walls")]
        [Label("Wall hit update event")]
        [SerializeField]
        protected UnityEvent<WallHitData> _wallHitDataUpdate;

        #endregion

        #region Fields

        protected WallHitData _data;

        protected float lengthConvertionRate = 100f;
        protected float positionOffsetConvertionRate = 100f;

        #endregion

        #region Properties

        public LayerMask whatIsWall { get { return _whatIsWall; } set { _whatIsWall = value; } }

        // All this convertions are made to make life easier on inspector
        protected float UpperRightLengthConverted => _upperRightDetectionLength / lengthConvertionRate;
        protected float UpperLeftLengthConverted => _upperLeftDetectionLength / lengthConvertionRate;

        protected float LowerRightLengthConverted => _lowerRightDetectionLength / lengthConvertionRate;
        protected float LowerLeftLengthConverted => _lowerLeftDetectionLength / lengthConvertionRate;

        protected float CenterRightLengthConverted => _centerRightDetectionLength / lengthConvertionRate;
        protected float CenterLeftLengthConverted => _centerLeftDetectionLength / lengthConvertionRate;

        // Positioning offset convertions for code legibility
        protected float UpperRightPositionXOffset => _upperRightPositionXOffset / positionOffsetConvertionRate;
        protected float UpperRightPositionYOffset => _upperRightPositionYOffset / positionOffsetConvertionRate;

        protected float UpperLeftPositionXOffset => _upperLeftPositionXOffset / positionOffsetConvertionRate;
        protected float UpperLeftPositionYOffset => _upperLeftPositionYOffset / positionOffsetConvertionRate;

        protected float LowerRightPositionXOffset => _lowerRightPositionXOffset / positionOffsetConvertionRate;
        protected float LowerRightPositionYOffset => _lowerRightPositionYOffset / positionOffsetConvertionRate;

        protected float LowerLeftPositionXOffset => _lowerLeftPositionXOffset / positionOffsetConvertionRate;
        protected float LowerLeftPositionYOffset => _lowerLeftPositionYOffset / positionOffsetConvertionRate;

        #endregion

        #region Getters

        /// <summary>
        /// Duh... the wall hit.
        /// </summary>
        /// <returns> true if... hitting a wall! </returns>
        public bool hittingWall => _hittingWall;
        public WallHitData data => _data;
        public UnityEvent<WallHitData> WallHitDataUpdate => _wallHitDataUpdate;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (_wallHitCollider == null) _wallHitCollider = GetComponent<Collider2D>();
        }

        protected virtual void FixedUpdate()
        {
            CheckWallHitting();
        }

        #endregion

        /// <summary>
        /// Casts rays to determine if character is grounded.
        /// </summary>
        /// <returns> true if grounded </returns>
        public void CheckWallHitting()
        {
            WallHitData data = new WallHitData();

            CastPositions positions = CalculatePositions(_wallHitCollider.bounds.center, _wallHitCollider.bounds.extents);

            if (_checkUpperRight)
            {
                CastFromPosition(positions.upperRight, Vector2.right, UpperRightLengthConverted, ref data.upperRight, ref data.upperRightHitAngle);
            }

            if (_checkLowerRight)
            {
                CastFromPosition(positions.lowerRight, Vector2.right, LowerRightLengthConverted, ref data.lowerRight, ref data.lowerRightHitAngle);
            }

            if (_checkCenterRight)
            {
                CastFromPosition(positions.centerRight, Vector2.right, CenterRightLengthConverted, ref data.centerRight, ref data.centerRightHitAngle);
            }

            if (_checkUpperLeft)
            {
                CastFromPosition(positions.upperLeft, Vector2.left, UpperLeftLengthConverted, ref data.upperLeft, ref data.upperLeftHitAngle);
            }

            if (_checkLowerLeft)
            {
                CastFromPosition(positions.lowerLeft, Vector2.left, LowerLeftLengthConverted, ref data.lowerLeft, ref data.lowerLeftHitAngle);
            }

            if (_checkCenterLeft)
            {
                CastFromPosition(positions.centerLeft, Vector2.left, CenterLeftLengthConverted, ref data.centerLeft, ref data.centerLeftHitAngle);
            }

            UpdateWallHittingStatus(data);
        }

        /// <summary>
        /// Executes the cast and updates the wall hit status.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <param name="datahit"></param>
        /// <param name="dataAngle"></param>
        protected void CastFromPosition(Vector2 position, Vector2 direction, float distance, ref RaycastHit2D datahit, ref float dataAngle)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, _whatIsWall);
            DebugCast(position, direction * distance, hit);

            if (hit)
            {
                if (!string.IsNullOrEmpty(_tagToIgnore) && hit.collider && hit.collider.CompareTag(_tagToIgnore))
                {
                    return;
                }
                datahit = hit;
                dataAngle = Vector2.Angle(position, hit.point);
            }
        }

        /// <summary>
        /// Updates wall hitting status based on data parameter.
        /// This will send an UnityEvent<WallHitData>
        /// </summary>
        /// <param name="wallHitStatusUpdate"></param>
        public void UpdateWallHittingStatus(WallHitData data)
        {
            this._data = data;
            _wallHitDataUpdate.Invoke(data);
        }

        /// <summary>
        /// Calculates positions where to cast from based on collider properties.
        /// </summary>
        /// <param name="center"> Stands for the collider center as a Vector2 </param>
        /// <param name="extents"> Stands for the collider extents as a Vector 2 </param>
        /// <returns></returns>
        protected CastPositions CalculatePositions(Vector2 center, Vector2 extents)
        {
            Vector2 upperRightPos = center + new Vector2(extents.x + UpperRightPositionXOffset, extents.y + UpperRightPositionYOffset);
            Vector2 lowerRightPos = center + new Vector2(extents.x + LowerRightPositionXOffset, -extents.y + LowerRightPositionYOffset);
            Vector2 centerRightPos = center + new Vector2(extents.x, 0);
            Vector2 upperLeftPos = center + new Vector2(-extents.x - UpperLeftPositionXOffset, extents.y + UpperLeftPositionYOffset);
            Vector2 lowerLeftPos = center + new Vector2(-extents.x - LowerLeftPositionXOffset, -extents.y + LowerLeftPositionYOffset);
            Vector2 centerLeftPos = center + new Vector2(-extents.x, 0);
            return new CastPositions(upperRightPos, lowerRightPos, centerRightPos, upperLeftPos, lowerLeftPos, centerLeftPos);
        }


        /// <summary>
        /// Debugs the ground check.
        /// </summary>
        protected void DebugCast(Vector2 start, Vector2 dir, RaycastHit2D hit)
        {
            if (!_debugOn) return;
            Debug.DrawRay(start, dir, hit.collider ? Color.red : Color.green);
        }

        /// <summary>
        /// Represents positions where to RayCast from
        /// </summary>
        protected struct CastPositions
        {
            public Vector2 upperRight;
            public Vector2 lowerRight;
            public Vector2 centerRight;
            public Vector2 upperLeft;
            public Vector2 lowerLeft;
            public Vector2 centerLeft;

            public CastPositions(Vector2 upperRightPos, Vector2 lowerRightPos, Vector2 centerRightPos, Vector2 upperLeftPos, Vector2 lowerLeftPos, Vector2 centerLeftPos)
            {
                upperRight = upperRightPos;
                lowerRight = lowerRightPos;
                centerRight = centerRightPos;
                upperLeft = upperLeftPos;
                lowerLeft = lowerLeftPos;
                centerLeft = centerLeftPos;
            }
        }
    }
}
