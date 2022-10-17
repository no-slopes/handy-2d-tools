
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management.Booting;
using UnityEditor;
using UnityEngine;

namespace H2DT.Audio
{
    public abstract class AudioDictionary<T0> : SingleScriptaBootable<AudioDictionary<T0>>
    {
        #region Inspector

        [Header("Audio Dictionary")]
        [Space]
        [SerializeField]
        private List<HandyAudioClip<T0>> _list = new List<HandyAudioClip<T0>>();

        #endregion

        #region Fields

        private Dictionary<T0, AudioClip> _dictionary = new Dictionary<T0, AudioClip>();

        #endregion

        #region Properties

        protected Dictionary<T0, AudioClip> dictionary => _dictionary;

        #endregion

        #region Booting

        public override async Task BootableBoot()
        {
            await base.BootableBoot();

            await Task.Run(() =>
            {
                _dictionary = new Dictionary<T0, AudioClip>();
                _list.ForEach(item => _dictionary.Add(item.audio, item.audioClip));
            });
        }

        #endregion

        #region Logic

        public static AudioClip GetClip(T0 audio)
        {
            if (instance != null && instance.dictionary.ContainsKey(audio))
            {
                return instance.dictionary[audio];
            }

            return null;
        }

        public static void TryGetClip(T0 audio, out AudioClip clip)
        {
            clip = null;
            if (instance != null)
                instance.dictionary.TryGetValue(audio, out clip);
        }

        #endregion
    }
}