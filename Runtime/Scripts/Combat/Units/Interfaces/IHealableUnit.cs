using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.Combat.Units
{
    public interface IHealableUnit
    {
        void Heal(float amount);
    }
}
