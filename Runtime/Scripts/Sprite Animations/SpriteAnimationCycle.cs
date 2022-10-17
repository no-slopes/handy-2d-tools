using System;
using System.Collections.Generic;
using UnityEngine;

namespace H2DT.SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationCycle
    {
        [SerializeField]
        protected List<SpriteAnimationFrame> _frames;

        public SpriteAnimationCycle()
        {
            _frames = new List<SpriteAnimationFrame>();
        }

        public List<SpriteAnimationFrame> Frames => _frames;
        public int FrameCount => _frames.Count;
    }
}