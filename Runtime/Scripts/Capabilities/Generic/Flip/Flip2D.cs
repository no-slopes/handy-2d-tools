using H2DT.Enums;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Generic
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Movement/Flip2D")]
    public class Flip2D : HandyComponent
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [Header("Horizontal")]
        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        [Space]
        protected FlipStrategy _horizontalFlipStrategy = FlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected HorizontalDirection _horizontalStartingDirection = HorizontalDirection.Right;

        [Header("Vertical")]
        [Tooltip("If the game object should be flipped scaling negatively on X axis or rotating Y axis 180º")]
        [SerializeField]
        [Space]
        protected FlipStrategy _verticalFlipStrategy = FlipStrategy.Rotating;

        [Tooltip("Use this to set wich direction GameObject should start flipped towards.")]
        [SerializeField]
        protected VerticalDirection _verticalStartingDirection = VerticalDirection.Down;

        // Events

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<HorizontalDirection> _facingDirectionUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction sign (-1 left or 1 right) this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<float> _facingDirectionSignUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<VerticalDirection> _verticalDirectionUpdate;

        [Foldout("Flip Events:")]
        [Space]
        [Tooltip("You can use these to directly set listeners about wich horizontal direction sign (-1 down or 1 up) this GameObject is flipped towards.")]
        [SerializeField]
        protected UnityEvent<float> _verticalDirectionSignUpdate;

        #region Fields

        protected bool _locked = false;

        #endregion

        #region Properties

        public HorizontalDirection _currentHorizontalDirection { get; protected set; }
        public float _currentHorizontalDirectionSign { get; protected set; }

        public VerticalDirection _currentVerticalDirection { get; protected set; }
        public float _currentVerticalDirectionSign { get; protected set; }

        #endregion

        #region Getters

        public UnityEvent<HorizontalDirection> FacingDirectionUpdate => _facingDirectionUpdate;
        public UnityEvent<float> FacingDirectionSignUpdate => _facingDirectionSignUpdate;

        public UnityEvent<VerticalDirection> VerticalDirectionUpdate => _verticalDirectionUpdate;
        public UnityEvent<float> VerticalDirectionSignUpdate => _verticalDirectionSignUpdate;

        public bool locked => _locked;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            InitialHorizontalFlip();
            InitialVerticalFlip();
        }

        #endregion

        #region Logic

        /// <summary>
        /// Evaluates if the game object can be flipped based on subjectDirection and if so, performs it.
        /// </summary>
        /// <param name="subjectDirection"></param>
        public virtual void EvaluateAndFlipHorizontally(float subjectDirection)
        {

            if (!ShouldFlipHorizontally(subjectDirection)) return;

            FlipHorizontally();
        }


        /// <summary>
        /// Evaluates if the game object can be flipped based on subjectDirection and if so, performs it.
        /// </summary>
        /// <param name="subjectDirection"></param>
        public virtual void EvaluateAndFlipVertically(float subjectDirection)
        {

            if (!ShouldFlipVertically(subjectDirection)) return;

            FlipVertically();
        }

        /// <summary>
        /// Flips character horizontally based on current horizontal flip strategy
        /// and current horizontal direction.
        /// </summary>
        public virtual void FlipHorizontally()
        {
            UpdateHorizontalDirection(_currentHorizontalDirectionSign * -1);

            switch (_horizontalFlipStrategy)
            {
                case FlipStrategy.Rotating:
                    transform.Rotate(0f, -180f, 0f);
                    break;
                case FlipStrategy.Scaling:
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    break;
                case FlipStrategy.Sprite:
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                    break;
            }
        }

        /// <summary>
        /// Flips character vertically based on current vertical flip strategy
        /// and current vertical direction.
        /// </summary>
        public virtual void FlipVertically()
        {
            UpdateVerticalDirection(_currentVerticalDirectionSign * -1);

            switch (_verticalFlipStrategy)
            {
                case FlipStrategy.Rotating:
                    transform.Rotate(-180f, 0f, 0f);
                    break;
                case FlipStrategy.Scaling:
                    transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                    break;
                default:
                    transform.Rotate(-180f, 0f, 0f);
                    break;
                case FlipStrategy.Sprite:
                    _spriteRenderer.flipY = !_spriteRenderer.flipY;
                    break;
            }
        }

        protected virtual void UpdateHorizontalDirection(float directionSign)
        {
            _currentHorizontalDirectionSign = directionSign;
            _currentHorizontalDirection = _currentHorizontalDirectionSign > 0 ? HorizontalDirection.Right : HorizontalDirection.Left;

            FacingDirectionUpdate.Invoke(_currentHorizontalDirection);
            FacingDirectionSignUpdate.Invoke(_currentHorizontalDirectionSign);
        }

        protected virtual void UpdateVerticalDirection(float directionSign)
        {
            _currentVerticalDirectionSign = directionSign;
            _currentVerticalDirection = _currentVerticalDirectionSign < 0 ? VerticalDirection.Down : VerticalDirection.Up;

            VerticalDirectionUpdate.Invoke(_currentVerticalDirection);
            VerticalDirectionSignUpdate.Invoke(_currentVerticalDirectionSign);
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialHorizontalFlip()
        {
            if (_horizontalStartingDirection == HorizontalDirection.Right || _horizontalStartingDirection == HorizontalDirection.None)
            {
                switch (_horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (transform.rotation.y != 0f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.x < 0)
                            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                        break;
                    case FlipStrategy.Sprite:
                        _spriteRenderer.flipX = false;
                        break;
                }
                UpdateHorizontalDirection(1);
                return;
            }

            if (_horizontalStartingDirection == HorizontalDirection.Left)
            {
                switch (_horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (Mathf.Abs(transform.rotation.y) != 180f)
                            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.x > 0)
                            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                        break;
                    case FlipStrategy.Sprite:
                        _spriteRenderer.flipX = true;
                        break;
                }
                UpdateHorizontalDirection(-1);
                return;
            }
        }

        /// <summary>
        /// Executes an initial Flip of the GameObject
        /// based on the startingDirection chosen on
        /// inspector.
        /// </summary>
        protected virtual void InitialVerticalFlip()
        {
            if (_verticalStartingDirection == VerticalDirection.Down || _verticalStartingDirection == VerticalDirection.None)
            {
                switch (_verticalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (transform.rotation.x != 0f)
                            transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.y < 0)
                            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                        break;
                    case FlipStrategy.Sprite:
                        _spriteRenderer.flipY = false;
                        break;
                }
                UpdateVerticalDirection(-1);
                return;
            }

            if (_verticalStartingDirection == VerticalDirection.Up)
            {
                switch (_horizontalFlipStrategy)
                {
                    case FlipStrategy.Rotating:
                        if (Mathf.Abs(transform.rotation.x) != 180f)
                            transform.rotation = Quaternion.Euler(180f, 0f, 0f);
                        break;
                    case FlipStrategy.Scaling:
                        if (transform.localScale.y > 0)
                            transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
                        break;
                    case FlipStrategy.Sprite:
                        _spriteRenderer.flipY = true;
                        break;
                }
                UpdateVerticalDirection(1);
                return;
            }
        }

        /// <summary>
        /// Evaluates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlipHorizontally(float subjectDirection)
        {
            // Debug.Log($"{subjectDirection} | {currentHorizontalDirectionSign}");
            return subjectDirection > 0 && _currentHorizontalDirectionSign < 0 || subjectDirection < 0 && _currentHorizontalDirectionSign > 0;
        }

        /// <summary>
        /// Evaluates if GameObject should be Flipped
        /// </summary>
        /// <param name="subjectDirection"></param>
        /// <returns></returns>
        protected virtual bool ShouldFlipVertically(float subjectDirection)
        {
            return subjectDirection > 0 && _currentVerticalDirectionSign < 0 || subjectDirection < 0 && _currentVerticalDirectionSign > 0;
        }

        public void Lock(bool shouldLock)
        {
            _locked = shouldLock;
        }

        #endregion
    }

    public enum FlipStrategy
    {
        Scaling,
        Rotating,
        Sprite,
    }
}
