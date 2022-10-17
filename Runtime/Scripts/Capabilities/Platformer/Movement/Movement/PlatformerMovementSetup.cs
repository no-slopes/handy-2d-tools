using H2DT.Capabilities;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;

namespace H2DT.Capabilities.Platforming
{

    [CreateAssetMenu(fileName = "New PlatformerMovementSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Movement")]
    public class PlatformerMovementSetup : Learnable
    {

        [Header("Movement Setup")]
        [Tooltip("The idle animation")]
        [Space]
        [SerializeField]
        protected SimpleSpriteAnimation _idleAnimation;

        [Tooltip("The Running animation")]
        [Space]
        [SerializeField]
        protected SimpleSpriteAnimation _runningAnimation;

        [Tooltip("The natural speed on X axis.")]
        [SerializeField]
        [Space]
        protected float _xSpeed = 10f;

        [Tooltip("The natural speed on X axis.")]
        [SerializeField]
        [Space]
        protected float _xMaxExceedingSpeed = 12f;

        [Header("Grounded Acceleration")]
        [Space]
        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [Range(0f, 20f)]
        protected float _groundedAcceleration = 7f;

        [SerializeField]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [Range(0f, 20f)]
        [Space]
        protected float _groundedDeceleration = 7f;

        [Header("On Air Acceleration")]
        [Space]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [Range(0f, 20f)]
        [SerializeField]
        protected float _onAirAcceleration = 7f;

        [SerializeField]
        [Space]
        [Tooltip("Higher this value is, sooner the character will reach the max speed coming from 0 velocity.")]
        [Range(0f, 20f)]
        protected float _onAirDeceleration = 7f;

        [SerializeField]
        [Space]
        [Range(0f, 1f)]
        protected float _power = 0.9f;

        [Header("Resistances")]
        [Space]
        [SerializeField]
        [Range(0f, 2f)]
        private float _friction = 1;

        [Header("Corners")]
        [Space]
        [SerializeField]
        private bool _helpOnCorners = false;

        [SerializeField]
        [Range(0f, 10f)]
        private float _cornerPushForce = 4f;


        #region Getters

        public SimpleSpriteAnimation idleAnimation => _idleAnimation;
        public SimpleSpriteAnimation runningAnimation => _runningAnimation;

        public float xSpeed => _xSpeed;
        public float xMaxExceedingSpeed => _xMaxExceedingSpeed;

        public float groundedAcceleration => _groundedAcceleration;
        public float groundedDeceleration => _groundedDeceleration;
        public float onAirAcceleration => _onAirAcceleration;
        public float onAirDeceleration => _onAirDeceleration;

        public float power => _power;
        public float friction => _friction;

        public bool helpOnCorners => _helpOnCorners;
        public float cornerPushForce => _cornerPushForce;

        #endregion
    }

}
