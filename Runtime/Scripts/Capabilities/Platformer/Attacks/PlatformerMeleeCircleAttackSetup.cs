using H2DT.Capabilities;
using H2DT.Combat.Units;
using H2DT.NaughtyAttributes;
using H2DT.SpriteAnimations;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Capabilities.Platforming
{

    public abstract class PlatformerMeleeCircleAttackSetup : Learnable
    {
        #region Inspector

        [Header("Simple Melee Attack Setup")]
        [Tooltip("The animation associated with the attack.")]
        [SerializeField]
        [Space]
        protected SpriteAnimation _animation;

        [Tooltip("The amount of damage that should be applied each hit.")]
        [SerializeField]
        [Space]
        protected float _damagePerHit = 100f;

        [Tooltip("The total radius of the attack.")]
        [SerializeField]
        protected float _radius = 1f;

        [Tooltip("The layer mask of collisions to the attack.")]
        [SerializeField]
        [Space]
        protected LayerMask _attackMask;

        [Tooltip("Tag that will be used to filter the attack.")]
        [Tag]
        [SerializeField]
        protected string _attackTag;

        [Foldout("Attack Events")]
        [SerializeField]
        [Space]
        protected UnityEvent _attackPerformed;

        [SerializeField]
        [Space]
        protected UnityEvent<GameObject> _hit;

        #endregion

        #region Properties

        public SpriteAnimation Animation => _animation;

        public float damagePerHit { get { return _damagePerHit; } set { _damagePerHit = value; } }
        public float radius { get { return _radius; } set { _radius = value; } }
        public LayerMask attackMask { get { return _attackMask; } set { _attackMask = value; } }
        public string attackTag { get { return _attackTag; } set { _attackTag = value; } }

        public UnityEvent AttackPerformed => _attackPerformed;
        public UnityEvent<GameObject> Hit => _hit;

        #endregion  

        #region Abstractions

        public abstract void Perform(Transform originPoint);
        protected abstract void DealDamage(IDamageableUnit damageableUnit, float damageAmount);

        #endregion
    }

}
