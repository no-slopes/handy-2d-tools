using UnityEngine;
using System.Collections.Generic;
using System;
using H2DT.SpriteAnimations.Handlers;

namespace H2DT.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Combo Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Combo Sprite Animation")]
    [Serializable]
    public class ComboSpriteAnimation : SpriteAnimation
    {

        #region Inspector

        [SerializeField]
        [Space]
        protected List<SpriteAnimationCycle> _cycles;

        #endregion

        #region Fields

        protected List<SpriteAnimationFrame> _frameListBuffer = new List<SpriteAnimationFrame>();

        #endregion

        #region Getters

        public List<SpriteAnimationCycle> Cycles => _cycles;

        public List<SpriteAnimationFrame> Frames => GetAllFrames();

        #endregion

        public override List<SpriteAnimationFrame> AllFrames => Frames;
        public override Type handlerType => typeof(ComboSpriteAnimationHandler);

        protected List<SpriteAnimationFrame> GetAllFrames()
        {
            _frameListBuffer.Clear();

            foreach (var cycle in _cycles)
            {
                foreach (var frame in cycle.Frames)
                {
                    _frameListBuffer.Add(frame);
                }
            }

            return _frameListBuffer;
        }

    }
}