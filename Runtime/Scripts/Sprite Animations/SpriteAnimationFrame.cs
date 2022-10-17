using UnityEngine;
using System;

namespace H2DT.SpriteAnimations
{
    [Serializable]
    public class SpriteAnimationFrame
    {
        #region Setup

        [SerializeField]
        protected int _id;

        [SerializeField]
        protected Sprite _sprite;

        [SerializeField]
        protected string _name;

        #endregion

        #region Getters

        public int Id => _id;
        public Sprite Sprite => _sprite;
        public string Name => _name;

        #endregion
    }
}