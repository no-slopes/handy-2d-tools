using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Combat.Units
{
    public interface ICombatUnit
    {
        float maxHealth { get; }
        float currentHealth { get; }
        bool alive { get; }

        UnityEvent cameToLife { get; }
        UnityEvent died { get; }
        UnityEvent<float> tookDamage { get; }
        UnityEvent<float> healed { get; }
    }
}
