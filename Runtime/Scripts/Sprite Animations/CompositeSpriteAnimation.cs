using UnityEngine;
using System.Collections.Generic;
using System;
using H2DT.NaughtyAttributes;
using System.Linq;
using H2DT.SpriteAnimations.Handlers;

namespace H2DT.SpriteAnimations
{
    [CreateAssetMenu(fileName = "Composite Sprite Animation", menuName = "Handy 2D Tools/Sprite Animator/Composite Sprite Animation")]
    [Serializable]
    public class CompositeSpriteAnimation : SpriteAnimation
    {

        #region Editor

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        [Header("Antecipation")]
        [SerializeField]
        protected bool _hasAntecipation = false;

        /// <summary>
        /// The Antecipation frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _antecipationCycle = new SpriteAnimationCycle();

        /// <summary>
        /// If the core should be looped. In case this is true, the method StopCoreLoop() should
        /// be called to stop the loop and this has to be done manually
        /// </summary>
        [Header("Core")]
        [SerializeField]
        [InfoBox("Note that if you mark the core as loopable you must tell the animation when to leave it manually. Otherwise it will loop untill other animation starts playing. Refer to documentation for more information.", EInfoBoxType.Warning)]
        [Space]
        protected bool _loopableCore = false;

        /// <summary>
        /// The core frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _coreCycle = new SpriteAnimationCycle();

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        [Header("Recovery")]
        [SerializeField]
        protected bool _hasRecovery = false;

        /// <summary>
        /// The Recovery frames
        /// </summary>
        /// <typeparam name="SpriteAnimationFrame"></typeparam>
        [SerializeField]
        protected SpriteAnimationCycle _recoveryCycle = new SpriteAnimationCycle();

        #endregion


        #region Getters

        /// <summary>
        /// If the animation has antecipation frames
        /// </summary>
        public bool HasAntecipation => _hasAntecipation;

        /// <summary>
        /// The antecipation frames
        /// </summary>
        public SpriteAnimationCycle AntecipationCycle => _antecipationCycle;

        /// <summary>
        /// The core frames
        /// </summary>
        public SpriteAnimationCycle CoreCycle => _coreCycle;

        /// <summary>
        /// If the core should loop. Note that if this is true, the method StopCoreLoop() should be called to stop the loop and this has to be done manually
        /// </summary>
        public bool LoopableCore => _loopableCore;

        /// <summary>
        /// If the animation has recovery frames
        /// </summary>
        public bool HasRecovery => _hasRecovery;

        /// <summary>
        /// The recovery frames
        /// </summary>
        public SpriteAnimationCycle RecoveryCycle => _recoveryCycle;

        #endregion

        /// <summary>
        /// All the frames in a single list
        /// </summary>
        public override List<SpriteAnimationFrame> AllFrames => GetAllFrames();


        /// <returns> All animation frames in a single list </returns>
        protected List<SpriteAnimationFrame> GetAllFrames()
        {
            return _antecipationCycle.Frames.Concat(_coreCycle.Frames).Concat(_recoveryCycle.Frames).ToList();
        }

        public override Type handlerType => typeof(CompositeSpriteAnimationHandler);
    }
}