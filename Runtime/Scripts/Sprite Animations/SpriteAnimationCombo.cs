using System;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationCombo
    {
        [SerializeField]
        protected List<SpriteAnimationCycle> _cycles = new List<SpriteAnimationCycle>();

        public SpriteAnimationCombo()
        {
            _cycles = new List<SpriteAnimationCycle>();
        }
    }
}