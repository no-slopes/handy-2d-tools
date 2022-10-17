using System.Collections;
using System.Collections.Generic;
using H2DT;
using UnityEngine;

namespace H2DT.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class HandyAudioSource : HandyComponent
    {
        #region Inspector

        [Header("Handling")]
        [Space]
        [SerializeField]
        private AudioHandler _audioHandler;

        #endregion

        #region Fields

        private AudioSource _audioSource;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponent<AudioSource>(ref _audioSource);
            DefineSourceOutput();
        }

        protected virtual void OnEnable()
        {
            _audioHandler.playRequest.AddListener(OnPlayRequest);
            _audioHandler.playOneShotRequest.AddListener(OnOneShotRequest);
            _audioHandler.stopRequest.AddListener(OnStopRequest);
        }

        protected virtual void OnDisable()
        {
            _audioHandler.playRequest.RemoveListener(OnPlayRequest);
            _audioHandler.playOneShotRequest.RemoveListener(OnOneShotRequest);
            _audioHandler.stopRequest.RemoveListener(OnStopRequest);
        }

        #endregion

        #region Logic

        protected virtual void DefineSourceOutput()
        {
            _audioSource.outputAudioMixerGroup = _audioHandler.audioMixerGroup;
        }

        protected virtual void OnStopRequest()
        {
            _audioSource.Stop();
        }

        protected virtual void OnPlayRequest(AudioClip audioClip)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }

        protected virtual void OnOneShotRequest(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        #endregion
    }
}
