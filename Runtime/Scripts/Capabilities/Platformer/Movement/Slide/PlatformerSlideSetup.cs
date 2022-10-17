using H2DT.Capabilities;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{

    [CreateAssetMenu(fileName = "New PlatformerSlideSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Slide")]
    public class PlatformerSlideSetup : Learnable
    {

        [Header("Slide Setup")]
        [Tooltip("The ability's animation")]
        [Space]
        [SerializeField]
        protected CompositeSpriteAnimation _animation;

        [Tooltip("The speed wich will be applyed to X axis during slide.")]
        [SerializeField]
        protected float _xSpeed = 20f;

        [Tooltip("Time in seconds of the slide duration.")]
        [SerializeField]
        protected float _duration = 1f;

        [Tooltip("Minimun time in seconds between slidees.")]
        [SerializeField]
        protected float _delay = 1f;

        [Tooltip("In case character is no longer grounded while performing slide, the slide is stoped.")]
        [SerializeField]
        protected bool _stopWhenNotGrounded = true;

        [Tooltip("Layers that should be considered ceiling")]
        [SerializeField]
        protected LayerMask _whatIsCeiling;

        [Tooltip("Ray cast length while performing to detect ceilings")]
        [SerializeField]
        protected float _ceilingDetectionLength = 2f;

        [Foldout("Slide Events")]
        [Label("Slide Update")]
        [SerializeField]
        [Space]
        protected UnityEvent<bool> _slideUpdate;

        #region Properties

        public CompositeSpriteAnimation Animation => _animation;
        public float xSpeed { get { return _xSpeed; } set { _xSpeed = value; } }
        public float duration { get { return _duration; } set { _duration = value; } }
        public float delay { get { return _delay; } set { _delay = value; } }
        public bool stopWhenNotGrounded { get { return _stopWhenNotGrounded; } set { _stopWhenNotGrounded = value; } }
        public LayerMask whatIsCeiling { get { return _whatIsCeiling; } set { _whatIsCeiling = value; } }
        public float ceilingDetectionLength { get { return _ceilingDetectionLength; } set { _ceilingDetectionLength = value; } }

        public UnityEvent<bool> SlideUpdate => _slideUpdate;

        #endregion
    }

}
