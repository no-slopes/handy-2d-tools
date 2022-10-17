using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Generics.Transitions;
using H2DT.Management.Booting;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Gameplay
{
    [CreateAssetMenu(fileName = "Gameplay Handler", menuName = "Handy 2D Tools/Management/Gameplay/Gameplay Handler")]
    public class GameplayHandler : ScriptableObject, IBootable
    {
        #region Inspector     

        [Header("Configuration")]
        [Space]
        [Range(0, 0.5f)]
        [SerializeField]
        protected float _freezeBeforePauseDuration = 0;

        [SerializeField]
        private bool _shouldFreezeWhenIdle;

        [Header("Session Events")]
        [Space]
        [SerializeField]
        public UnityEvent<int> GameplayTimeUpdate;

        #endregion

        #region Fields

        private Dictionary<GameplayStateType, UnityEvent<bool>> _stateStatusEvents = new Dictionary<GameplayStateType, UnityEvent<bool>>();
        private Dictionary<GameplayStateType, UnityEvent> _stateRequestEvents = new Dictionary<GameplayStateType, UnityEvent>();

        public GameplayStateType currentStateType { get; set; }


        // Pause Subjects
        protected TransitionCommander<GameplayTransitionSubject> _gameplayTransitionsCommander;
        protected Dictionary<GameplayStateType, List<GameplayTransitionSubject>> _gameplayTransitionSubjects;

        #endregion

        #region Getters

        public bool shouldFreezeWhenIdle => _shouldFreezeWhenIdle;

        public float freezeBeforePauseDuration => _freezeBeforePauseDuration;
        public TransitionCommander<GameplayTransitionSubject> gameplayTransitionsCommander => _gameplayTransitionsCommander;

        public bool idle => currentStateType == GameplayStateType.Idle;
        public bool playing => currentStateType == GameplayStateType.Playing;
        public bool playingCutscene => currentStateType == GameplayStateType.Cutscene;
        public bool paused => currentStateType == GameplayStateType.Paused;
        public bool gameover => currentStateType == GameplayStateType.Gameover;

        #endregion

        #region Initializing

        public async Task BootableBoot()
        {
            await Task.Run(() =>
            {
                _gameplayTransitionsCommander = new TransitionCommander<GameplayTransitionSubject>();
                _gameplayTransitionSubjects = new Dictionary<GameplayStateType, List<GameplayTransitionSubject>>();
                _stateRequestEvents = new Dictionary<GameplayStateType, UnityEvent>();
                _stateStatusEvents = new Dictionary<GameplayStateType, UnityEvent<bool>>();

                foreach (GameplayStateType stateType in Enum.GetValues(typeof(GameplayStateType)))
                {
                    _stateStatusEvents.Add(stateType, new UnityEvent<bool>());
                    _stateRequestEvents.Add(stateType, new UnityEvent());
                    _gameplayTransitionSubjects.Add(stateType, new List<GameplayTransitionSubject>());
                }
            });


        }

        public async Task BootableDismiss()
        {
            await Task.Run(() =>
            {
                _gameplayTransitionsCommander = null;
                _gameplayTransitionSubjects = null;
            });
        }

        #endregion

        #region Requesting states

        /// <summary>
        /// Call this in oder to request the rest state.
        /// </summary>
        public void Rest()
        {
            _stateRequestEvents[GameplayStateType.Idle].Invoke();
        }

        /// <summary>
        /// Call this in oder to request the gameplay state. The manager is responsible to figure out if it should
        /// or not enter the state.
        /// </summary>
        public void RequestGameplay()
        {
            _stateRequestEvents[GameplayStateType.Playing].Invoke();
        }

        /// <summary>
        /// Call this in oder to request the pause state. The manager is responsible to figure out if it should
        /// or not enter the state.
        /// </summary>
        public void RequestPause()
        {
            _stateRequestEvents[GameplayStateType.Paused].Invoke();
        }

        /// <summary>
        /// Call this in oder to request the gameover state. The manager is responsible to figure out if it should
        /// or not enter the state.
        /// </summary>
        public void RequestGameOver()
        {
            _stateRequestEvents[GameplayStateType.Gameover].Invoke();
        }

        public void RequestCutscene()
        {
            _stateRequestEvents[GameplayStateType.Cutscene].Invoke();
        }

        #endregion

        #region Session

        public void AnnouncePlayedTime(int timeInSeconds)
        {
            GameplayTimeUpdate.Invoke(timeInSeconds);
        }

        #endregion

        #region Time Scale

        /// <summary>
        /// This will set time scale to 0.0f and freeze the game physics.
        /// You can provide a duration in seconds and a callback to be called when the time scale is set to 0.0f.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="Callback"></param>
        public async void Freeze(float duration = 0, UnityAction Callback = null)
        {
            await TransitionIntoTimeScale(0, duration, Callback);
        }

        /// <summary>
        /// This will set time scale to 1.0f and restabilish the game physics.
        /// You can provide a duration in seconds and a callback to be called when the time scale is set to 1.0f.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="Callback"></param>
        public async void Unfreeze(float duration = 0, UnityAction Callback = null)
        {
            await TransitionIntoTimeScale(1, duration, Callback);
        }

        /// <summary>
        /// This will transition into a new time scale over a duration. If the duration is 0, it will transition instantly.
        /// If a callback is provided, it will be called after the transition is complete.
        /// </summary>
        /// <param name="targetTimeScale"></param>
        /// <param name="duration"></param>
        /// <param name="Callback"></param>
        public async Task TransitionIntoTimeScale(float targetTimeScale, float duration, UnityAction Callback = null)
        {
            await ApplyTimeScaleTransition(targetTimeScale, duration, Callback);
        }

        /// <summary>
        /// The coroutine to apply time scale transition over a period of time and execute a callback when finished.
        /// </summary>
        /// <param name="targetTimeScale"></param>
        /// <param name="duration"></param>
        /// <param name="Callback"></param>
        /// <returns></returns>
        protected async Task ApplyTimeScaleTransition(float targetTimeScale, float duration, UnityAction Callback)
        {
            float currentTimeScale = Time.timeScale;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                AlterTimeScale(Mathf.Lerp(currentTimeScale, targetTimeScale, elapsedTime / duration));
                await Task.Yield();
            }

            AlterTimeScale(targetTimeScale);
            Callback?.Invoke();
        }

        /// <summary>
        /// Alters the time scale of the game.
        /// </summary>
        /// <param name="timeScale"></param>
        public void AlterTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        #endregion

        #region Transition Subjects

        /// <summary>
        /// Registers a transition Subject for a specific state.
        /// </summary>
        /// <param name="stateType"></param>
        /// <param name="subject"></param>
        public void RegisterTransitionSubject(GameplayStateType stateType, GameplayTransitionSubject subject)
        {
            _gameplayTransitionSubjects[stateType].Add(subject);
        }

        /// <summary>
        /// Unregisters a transition Subject from a specific state.
        /// </summary>
        /// <param name="stateType"></param>
        /// <param name="subject"></param>
        public void UnregisterTransitionSubject(GameplayStateType stateType, GameplayTransitionSubject subject)
        {
            _gameplayTransitionSubjects[stateType].Remove(subject);
        }

        /// <summary>
        /// Clears all transition subjects
        /// </summary>
        /// <param name="stateType"></param>
        public void ClearTransitionSubjects(GameplayStateType stateType)
        {
            _gameplayTransitionSubjects[stateType].Clear();
        }

        /// <summary>
        /// Gets all transition subjects for a given state
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public List<GameplayTransitionSubject> GetCurrentTransitionSubjects(GameplayStateType stateType)
        {
            return _gameplayTransitionSubjects[stateType];
        }

        #endregion

        #region State Status events

        public void TryGetStateStatusEvent(GameplayStateType stateType, out UnityEvent<bool> stateStatusEvent)
        {
            _stateStatusEvents.TryGetValue(stateType, out stateStatusEvent);
        }

        public UnityEvent<bool> GetStateStatusEvent(GameplayStateType stateType)
        {
            return _stateStatusEvents[stateType];
        }

        #endregion

        #region State Request Events

        public void TryGetStateRequestEvent(GameplayStateType stateType, out UnityEvent stateRequestEvent)
        {
            _stateRequestEvents.TryGetValue(stateType, out stateRequestEvent);
        }

        public UnityEvent GetStateRequestEvent(GameplayStateType stateType)
        {
            return _stateRequestEvents[stateType];
        }

        #endregion
    }
}
