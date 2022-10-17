using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Combat.Units
{
    [CreateAssetMenu(fileName = "New CombatUnitSetup", menuName = "Handy 2D Tools/Combat/Setups/CombatUnitSetup")]
    public class CombatUnitSetup : ScriptableObject
    {
        #region Inspector

        [Header("Health")]
        [Space]
        [SerializeField]
        protected float _maxHealth = 100;

        [SerializeField]
        protected float _startingHealth = 100;

        [Header("Spawning")]
        [Space]
        [SerializeField]
        protected bool _spawnAlive = true;

        [ShowIf("_spawnAlive")]
        [SerializeField]
        protected bool _spawnFullHealth = false;

        [Header("Healing")]
        [Space]
        [SerializeField]
        protected bool _canHeal = false;

        [Header("Vulnerability")]
        [Space]
        [SerializeField]
        protected bool _invulnerable = false;

        [Header("Events")]
        [Space]
        [SerializeField]
        protected UnityEvent _cameToLife;

        [SerializeField]
        protected UnityEvent _died;

        [SerializeField]
        protected UnityEvent<float> _tookDamage;

        [SerializeField]
        protected UnityEvent<float> _healed;

        #endregion

        #region Fields

        protected float _currentHealth = 100;

        #endregion

        #region Properties

        public float maxHealth { get => _maxHealth; set => _maxHealth = value; }
        public float startingHealth { get => _startingHealth; set => _startingHealth = value; }
        public bool spawnAlive { get => _spawnAlive; set => _spawnAlive = value; }
        public bool spawnFullHealth { get => _spawnFullHealth; set => _spawnFullHealth = value; }
        public bool canHeal { get => _canHeal; set => _canHeal = value; }
        public bool invulnerable { get => _invulnerable; set => _invulnerable = value; }

        public float currentHealth { get => _currentHealth; set => _currentHealth = value; }

        #endregion

        #region Getters

        public bool alive => _currentHealth > 0;

        public UnityEvent cameToLife => _cameToLife;
        public UnityEvent died => _died;
        public UnityEvent<float> tookDamage => _tookDamage;
        public UnityEvent<float> healed => _healed;

        #endregion       

        #region 

        /// <summary>
        /// Activates the invulnerability of the combat unit.
        /// </summary>
        public virtual void ActivateInvulnerability()
        {
            _invulnerable = true;
        }

        /// <summary>
        /// Deactivates the invulnerability of the combat unit.
        /// </summary>
        public virtual void DeactivateInvulnerability()
        {
            _invulnerable = false;
        }

        /// <summary>
        /// Toggles the invulnerability of the combat unit.
        /// </summary>
        public virtual void ToggleInvulnerability()
        {
            _invulnerable = !_invulnerable;
        }

        /// <summary>
        /// Activates unit's healing ability.
        /// </summary>
        public virtual void ActivateHealing()
        {
            _canHeal = true;
        }

        /// <summary>
        /// Deactivates unit's healing ability.
        /// </summary>
        public virtual void DeactivateHealing()
        {
            _canHeal = false;
        }

        /// <summary>
        /// Toggles unit's healing ability.
        /// </summary>
        public virtual void ToggleHealing()
        {
            _canHeal = !_canHeal;
        }

        #endregion
    }
}
