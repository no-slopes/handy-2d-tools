using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.Combat.Units
{
    public interface IDamager
    {
        void DealDamage(IDamageableUnit damageableUnit, float amount);
    }
}