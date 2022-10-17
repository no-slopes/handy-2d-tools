using H2DT.Combat.Units;
using H2DT.SpriteAnimations;
using UnityEngine;

namespace H2DT.Capabilities.Platforming
{

    [CreateAssetMenu(fileName = "New PlatformerComboCircleMeleeAttackSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Combo Circle Melee Attack")]
    public class PlatformerComboCircleMeleeAttackSetup : PlatformerMeleeCircleAttackSetup
    {
        [Header("Combo Attack")]
        [Space]
        [SerializeField]
        protected float _betweenCyclesInterruptionTime = 1f;

        [SerializeField]
        protected float _pushForce = 40f;

        #region Getters

        public new ComboSpriteAnimation Animation => _animation as ComboSpriteAnimation;
        public float BetweenCyclesInterruptionTime => _betweenCyclesInterruptionTime;
        public float PushForce => _pushForce;

        #endregion

        /// <summary>
        /// Performs a circle cast originating from the origin point and with the given radius.
        /// </summary>
        /// <param name="originPoint"></param>
        public override void Perform(Transform originPoint)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(originPoint.position, _radius, Vector2.zero, Mathf.Infinity, _attackMask);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag(_attackTag))
                {
                    IDamageableUnit damageable = hit.collider.GetComponent<IDamageableUnit>();
                    DealDamage(damageable, _damagePerHit);
                }
            }
        }

        /// <summary>
        /// Applies damage to the given damageable unit.
        /// </summary>
        /// <param name="damageableUnit"></param>
        /// <param name="damageAmount"></param>
        protected override void DealDamage(IDamageableUnit damageableUnit, float damageAmount)
        {
            if (damageableUnit == null) return;

            damageableUnit.TakeDamage(damageAmount);
        }
    }

}
