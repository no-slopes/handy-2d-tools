using System.Collections;
using System.Collections.Generic;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Combat.Units
{
    [AddComponentMenu("Handy 2D Tools/Combat/Units/CombatUnit")]
    public class CombatUnit : DocumentedComponent, ICombatUnit, IDamageableUnit, IHealableUnit
    {
        #region Inspector

        [SerializeField]
        protected CombatUnitSetup _setup;

        #endregion

        #region Fields

        #endregion

        #region Getters

        public float maxHealth { get { return _setup.maxHealth; } }
        public float currentHealth { get { return _setup.currentHealth; } }
        public bool alive { get { return _setup.alive; } }

        public UnityEvent cameToLife => _setup.cameToLife;
        public UnityEvent died => _setup.died;
        public UnityEvent<float> tookDamage => _setup.tookDamage;
        public UnityEvent<float> healed => _setup.healed;

        #endregion

        #region MonoBehaviour

        protected virtual void Awake()
        {
            Spawn();
        }

        #endregion

        #region Spawning

        /// <summary>
        /// Spawns the unit.
        /// </summary>
        protected void Spawn()
        {
            if (_setup.spawnAlive)
            {
                if (_setup.spawnFullHealth)
                {
                    ComeToLife(_setup.maxHealth);
                }
                else
                {
                    ComeToLife(_setup.startingHealth);
                }
            }
        }

        #endregion

        #region Health Handling

        /// <summary>
        /// If unit is not invulnerable, deduces the current health of the combat unit by the given amount and the healed event is invoked.
        /// If the current health is less than or equal to 0, the combat unit is considered dead and the died event is invoked.
        /// </summary>
        /// <param name="amount"></param>
        public virtual void TakeDamage(float amount)
        {
            if (_setup.invulnerable || !alive)
                return;

            _setup.currentHealth -= amount;

            _setup.tookDamage.Invoke(amount);

            if (_setup.currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// If unit can heal, increases the current health of the combat unit by the given amount and the healed event is invoked.
        /// If the current health is greater than the maximum health, the current health is set to the maximum health.
        /// </summary>
        /// <param name="amount"></param>
        public virtual void Heal(float amount)
        {
            if (!_setup.canHeal)
                return;

            _setup.currentHealth += amount;

            _setup.healed.Invoke(amount);

            if (_setup.currentHealth > _setup.maxHealth)
            {
                _setup.currentHealth = _setup.maxHealth;
            }
        }

        /// <summary>
        /// Sets the unit as alive and if fullHealth is true, the current health is set to the maximum health.
        /// The cameToLife event is invoked.
        /// </summary>
        /// <param name="healthAmount"></param>
        /// <param name="fullHealth"></param>
        protected virtual void ComeToLife(float healthAmount)
        {
            _setup.currentHealth = healthAmount;
            _setup.cameToLife.Invoke();
        }

        /// <summary>
        /// Sets the unit as alive and if fullHealth is true, the current health is set to the maximum health.
        /// The cameToLife event is invoked.
        /// </summary>
        /// <param name="healthAmount"></param>
        /// <param name="fullHealth"></param>
        protected virtual void ComeToLife()
        {
            ComeToLife(_setup.maxHealth);
        }

        /// <summary>
        /// Sets the unit as dead and the died event is invoked.
        /// </summary>
        protected virtual void Die()
        {
            _setup.died.Invoke();
        }

        #endregion

        #region DocumentedComponent

        protected override string docPath => "core/combat/unit.html";

        #endregion

    }
}
