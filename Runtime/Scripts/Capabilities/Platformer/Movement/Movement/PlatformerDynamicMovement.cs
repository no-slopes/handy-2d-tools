using UnityEngine;
using H2DT.NaughtyAttributes;
using H2DT.Capabilities;

namespace H2DT.Capabilities.Platforming
{
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Movement/PlatformerDynamicMovement")]
    public class PlatformerDynamicMovement : LearnableAbilityComponent<PlatformerMovementSetup>
    {

        #region Components

        protected Rigidbody2D _rb;

        #endregion

        #region Interfaces

        protected IPlaftormerGroundingProvider _groundingProvider;

        #endregion

        #region Fields

        protected float _defaultGravityScale;
        protected float _xDamper = 0;
        protected bool _grounded = false;
        protected bool _locked = false;

        #endregion

        #region Properties

        public float defaultGravityScale => _defaultGravityScale;

        protected float accelerationRatio => _grounded ? setup.groundedAcceleration : setup.onAirAcceleration;
        protected float decelerationRatio => _grounded ? setup.groundedDeceleration : setup.onAirDeceleration;

        #endregion

        #region Getters

        public float currentGravityScale => _rb.gravityScale;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();
            FindComponents();
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        protected void OnEnable()
        {
            SubscribeSeekers();
        }

        protected void OnDisable()
        {
            UnsubscribeSeekers();
        }

        #endregion

        #region Logic

        /// <summary>
        /// Moves character along X axis based on xSpeed    
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="directionSign"></param>
        public virtual void MoveHorizontally(float speed, float directionSign)
        {
            float targetSpeedX = speed * directionSign;

            float acceleration = Mathf.Abs(targetSpeedX) > 0.01f ? accelerationRatio : decelerationRatio;
            float speedDiff = targetSpeedX - _rb.velocity.x;

            float xForce = Mathf.Pow(Mathf.Abs(speedDiff) * acceleration, setup.power) * directionSign;

            _rb.AddForce(new Vector2(xForce, 0));

            if (_grounded && directionSign == 0)
            {
                float frictionAmount = Mathf.Min(Mathf.Abs(_rb.velocity.x), Mathf.Abs(setup.friction));
                frictionAmount *= Mathf.Sign(_rb.velocity.x);

                _rb.AddForce(new Vector2(-frictionAmount, 0), ForceMode2D.Impulse);
            }

            if (Mathf.Sign(targetSpeedX) < 0 && _rb.velocity.x < -Mathf.Abs(targetSpeedX))
            {
                _rb.velocity = new Vector2(-Mathf.Abs(targetSpeedX), _rb.velocity.y);
            }
            else if (Mathf.Sign(targetSpeedX) > 0 && _rb.velocity.x > Mathf.Abs(targetSpeedX))
            {
                _rb.velocity = new Vector2(Mathf.Abs(targetSpeedX), _rb.velocity.y);
            }
        }

        /// <summary>
        /// Moves character along X axis based on xSpeed   
        /// This will use the natural xSpeed set on inspector    
        /// </summary>
        /// <param name="directionSign"></param>
        public virtual void MoveHorizontally(float directionSign)
        {
            MoveHorizontally(setup.xSpeed, directionSign);
        }

        /// <summary>
        /// Pushs the character along X axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushHorizontally(float force, float directionSign)
        {
            _rb.AddForce(new Vector2(force * directionSign, _rb.velocity.y));
        }

        /// <summary>
        /// Applies vertical speed to the character
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="directionSign"></param>
        public virtual void MoveVertically(float speed, float directionSign)
        {
            Vector2 velocity = new Vector2(_rb.velocity.x, speed * directionSign);
            _rb.velocity = velocity;
        }

        /// <summary>
        /// Pushs the character along Y axis towards given direction sign using the amount of force given
        /// </summary>
        /// <param name="force"></param>
        /// <param name="directionSign"></param>
        public virtual void PushVertically(float force, float directionSign)
        {
            _rb.AddForce(new Vector2(0, force * directionSign));
        }

        /// <summary>
        /// Applies 0 to both velocity axis of the character
        /// Have in mind that the RigidBody2D will still move on Y axis if gravity is being applied
        /// </summary>
        public virtual void StopMovement()
        {
            _rb.velocity = new Vector2(0, _rb.velocity.y);
        }

        /// <summary>
        /// Changes the gravity scale of the character
        /// </summary>
        /// <param name="newGravityScale"></param>
        public virtual void ChangeGravityScale(float newGravityScale)
        {
            _rb.gravityScale = newGravityScale;
        }

        #endregion

        #region Updates

        /// <summary>
        /// Updates the grounded status
        /// </summary>
        /// <param name="newGrounding"></param>
        public void UpdateGrounding(bool newGrounding)
        {
            _grounded = newGrounding;
        }

        #endregion

        #region Update Seeking

        protected void FindComponents()
        {
            FindComponent<IPlaftormerGroundingProvider>(ref _groundingProvider);
        }

        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            _groundingProvider?.GroundingUpdate.AddListener(UpdateGrounding);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            _groundingProvider?.GroundingUpdate.RemoveListener(UpdateGrounding);
        }

        #endregion
    }
}
