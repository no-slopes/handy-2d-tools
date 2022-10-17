
using H2DT.Management.Booting;
using UnityEngine;

namespace H2DT.Audio
{
    [CreateAssetMenu(fileName = "Handy Audio Settings", menuName = "Handy 2D Tools/Audio/Handy Audio Settings")]
    public class HandyAudioSettings : ScriptaBooter
    {
        #region Inspector

        [Header("Volume Changing")]
        [Space]
        [SerializeField]
        private AnimationCurve _volumeChangingCurve;

        #endregion

        #region Getters

        public AnimationCurve volumeChangingCurve => _volumeChangingCurve;

        #endregion
    }
}