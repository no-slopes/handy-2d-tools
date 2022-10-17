using H2DT.Combat.Units;
using H2DT.NaughtyAttributes;
using UnityEngine;

namespace H2DT.Capabilities.Platforming
{
    [AddComponentMenu("Handy 2D Tools/Platformer/Abilities/Attacks/PlatformerSimpleMeleeAttack")]
    public class PlatformerSimpleMeleeAttack : Attack
    {
        #region Inspector

        [Header("Dependencies")]
        [InfoBox("If you prefer you can read the docs on how to feed this component directly through one of your scripts.")]
        [Space]
        [Tooltip("If you guarantee your GameObject has a component wich implements an IPlatformerSimpleMeleeAttackHandler you can mark this and it will subscribe to its events.")]
        [SerializeField]
        protected bool _seekHandler = false;

        [Tooltip("If you guarantee your GameObject has a component wich implements an IAttackLocker you can mark this and it will subscribe to its events.")]
        [SerializeField]
        protected bool _seekLocker = false;

        [SerializeField]
        protected Transform _attackPoint;

        [SerializeField]
        protected float _attackRadius = 1f;

        [SerializeField]
        protected bool _oncePerEngagement = true;

        [SerializeField]
        protected LayerMask _attackMask;

        [Tag]
        [SerializeField]
        protected string _attackTag;

        #endregion

        #region Components

        protected IPlatformerSimpleMeleeAttackHandler _handler;
        protected IAttackLocker _locker;

        #endregion

        #region Fields

        #endregion

        #region Properties

        protected override bool CanEngage => !_engaged && !_locked;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponents();
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

        #region Attack Logic

        public void Perform()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(_attackPoint.position, _attackRadius, Vector2.zero, Mathf.Infinity, _attackMask);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag(_attackTag))
                {
                    // Debug.Log($"Hit {hit.collider.name}");
                }
            }
        }

        protected void DealDamage(IDamageableUnit damageableUnit, float damageAmount)
        {
            damageableUnit.TakeDamage(damageAmount);
        }

        #endregion

        #region Components

        /// <summary>
        /// Finds needed components
        /// </summary>
        protected void FindComponents()
        {
            SeekComponent<IPlatformerSimpleMeleeAttackHandler>(_seekHandler, ref _handler);
            SeekComponent<IAttackLocker>(_seekLocker, ref _locker);
        }


        /// <summary>
        /// Subscribes to events based on components wich implements
        /// the correct interfaces
        /// </summary>
        protected virtual void SubscribeSeekers()
        {
            _handler?.SimpleMeleeAttackRequest.AddListener(Request);
            _locker?.AttackLock.AddListener(Lock);
        }

        /// <summary>
        /// Unsubscribes from events
        /// </summary>
        protected virtual void UnsubscribeSeekers()
        {
            _handler?.SimpleMeleeAttackRequest.RemoveListener(Request);
            _locker?.AttackLock.RemoveListener(Lock);
        }


        #endregion

        #region Debug

        protected void OnDrawGizmosSelected()
        {
            if (_attackPoint == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_attackPoint.position, _attackRadius);
        }

        #endregion

        #region Documented Component

        protected override string docPath => "core/platformer/attacks/platformer-simple-melee-attack.html";

        #endregion
    }
}
