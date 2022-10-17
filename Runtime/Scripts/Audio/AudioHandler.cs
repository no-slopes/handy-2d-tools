using System.Threading.Tasks;
using H2DT.Management.Booting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using static H2DT.Utils.Math;

namespace H2DT.Audio
{
    [CreateAssetMenu(fileName = "Audio Handler", menuName = "Handy 2D Tools/Audio/Audio Handler")]
    public class AudioHandler : ScriptableObject, IBootable
    {
        protected static string SoundPrefPrefix = "HD2T_Sound_";

        #region Inpector

        [Header("Settings")]
        [Space]
        [SerializeField]
        private HandyAudioSettings _settings;

        [Header("Mixer")]
        [Space]
        [SerializeField]
        private AudioMixer _audioMixer;

        [SerializeField]
        private AudioMixerGroup _audioMixerGroup;

        [Tooltip("This must be an exposed parameter in the mixer group")]
        [SerializeField]
        private string _volumeExposedParam;

        [Header("UI Helpers")]
        [Space]
        [SerializeField]
        protected string _uiLabel;

        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<AudioClip> _playRequest;

        [SerializeField]
        private UnityEvent<AudioClip> _playOneShotRequest;

        [SerializeField]
        private UnityEvent _stopRequest;

        [SerializeField]
        private UnityEvent<float> _volumeChanged;

        #endregion

        #region Fields

        protected float _volume = 1;

        /// <summary>
        /// The normalized (0 - 1) volume.
        /// </summary>
        /// <value></value>
        public float volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _audioMixer.SetFloat(_volumeExposedParam, ConvertToMixerScale(_volume));
                PlayerPrefs.SetFloat(playerPrefsKey, _volume);
                _volumeChanged.Invoke(_volume);
            }
        }

        #endregion

        #region Properties

        protected string playerPrefsKey => SoundPrefPrefix + _audioMixerGroup.name;

        #endregion

        #region Getters

        public AudioMixer audioMixer => _audioMixer;
        public AudioMixerGroup audioMixerGroup => _audioMixerGroup;

        /// <summary>
        /// The text to represent this handler on UI
        /// </summary>
        public string uiLabel => _uiLabel;

        // Events
        public UnityEvent<AudioClip> playRequest => _playRequest;
        public UnityEvent<AudioClip> playOneShotRequest => _playOneShotRequest;
        public UnityEvent stopRequest => _stopRequest;
        public UnityEvent<float> volumeChanged => _volumeChanged;

        #endregion

        #region Booting

        public Task BootableBoot()
        {
            if (PlayerPrefs.HasKey(playerPrefsKey))
            {
                _volume = PlayerPrefs.GetFloat(playerPrefsKey);

                float converted = ConvertToMixerScale(_volume);
                _audioMixer.SetFloat(_volumeExposedParam, converted);
            }

            return Task.CompletedTask;
        }

        public Task BootableDismiss()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Requests

        public virtual void Stop()
        {
            _stopRequest.Invoke();
        }

        public virtual void Play(AudioClip audioClip)
        {
            if (audioClip == null) return;

            _playRequest.Invoke(audioClip);
        }

        public virtual void PlayOneShot(AudioClip audioClip)
        {
            if (audioClip == null) return;

            _playOneShotRequest.Invoke(audioClip);
        }

        #endregion

        #region Mixer

        /// <summary>
        /// Converts a normalized volume value into the mixed db system.
        /// </summary>
        /// <param name="normalized"></param>
        /// <returns></returns>
        protected virtual float ConvertToMixerScale(float normalized)
        {
            float curvedForMixer = _settings.volumeChangingCurve.Evaluate(normalized);
            return ConvertScale(curvedForMixer, 1, -80, 0);
        }

        #endregion
    }
}
