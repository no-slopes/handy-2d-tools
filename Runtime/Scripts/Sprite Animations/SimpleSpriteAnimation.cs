using UnityEngine;
using System.Collections.Generic;
using System;
using H2DT.SpriteAnimations.Handlers;

namespace H2DT.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Simple Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Simple Sprite Animation")]
    [Serializable]
    public class SimpleSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation should loop
        /// </summary>
        [SerializeField]
        [Space]
        protected bool _loop = false;

        /// <summary>
        /// The animation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _cycle = new SpriteAnimationCycle();

        #endregion  

        #region Getters

        public bool Loop => _loop;
        public SpriteAnimationCycle Cycle => _cycle;

        #endregion

        public override List<SpriteAnimationFrame> AllFrames => _cycle.Frames;
        public override Type handlerType => typeof(SimpleSpriteAnimationHandler);

    }
}