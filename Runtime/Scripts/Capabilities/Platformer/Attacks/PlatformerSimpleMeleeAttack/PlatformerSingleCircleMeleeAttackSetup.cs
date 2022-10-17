using H2DT.Combat.Units;
using H2DT.SpriteAnimations;
using UnityEngine;

namespace H2DT.Capabilities.Platforming
{

    [CreateAssetMenu(fileName = "New PlatformerSingleCircleMeleeAttackSetup", menuName = "Handy 2D Tools/Platformer/Abilities/Setups/Single Circle Melee Attack")]
    public class PlatformerSingleCircleMeleeAttackSetup : PlatformerMeleeCircleAttackSetup
    {

        #region Getters

        public new CompositeSpriteAnimation Animation => _animation as CompositeSpriteAnimation;

        #endregion

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

        protected override void DealDamage(IDamageableUnit damageableUnit, float damageAmount)
        {
            if (damageableUnit != null)
                damageableUnit.TakeDamage(damageAmount);
        }
    }

}
