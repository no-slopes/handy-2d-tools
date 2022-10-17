using UnityEngine;
using System.Collections.Generic;
using System;

namespace H2DT.SpriteAnimations
{

    public abstract class SpriteAnimation : ScriptableObject
    {

        #region Editor

        /// <summary>
        /// The name of the animation.
        /// </summary>
        [Header("Setup")]
        [SerializeField]
        [Space]
        protected string _name = "New Animation";

        /// <summary>
        /// Amount of frames per second.
        /// </summary>
        [SerializeField]
        protected int _fps = 6;

        #endregion

        #region Properties

        #endregion

        #region Getters

        // Setup
        /// <summary>
        /// The name of the animation.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// The amount of frames per second.
        /// </summary>
        public int FPS => _fps;

        #endregion

        #region Abstratctions

        public abstract List<SpriteAnimationFrame> AllFrames { get; }
        public abstract Type handlerType { get; }

        #endregion

    }
}