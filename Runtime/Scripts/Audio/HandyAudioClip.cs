using System;
using UnityEngine;

namespace H2DT.Audio
{
    [Serializable]
    public struct HandyAudioClip<T0>
    {
        #region Inspector

        [SerializeField]
        private T0 _audio;

        [SerializeField]
        private AudioClip _audioClip;

        #endregion

        #region Getters

        public T0 audio => _audio;
        public AudioClip audioClip => _audioClip;

        #endregion
    }
}