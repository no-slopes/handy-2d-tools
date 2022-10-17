using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management.Booting;
using H2DT.Management.Curtains;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace H2DT.Management.Scenes
{
    [CreateAssetMenu(fileName = "Scene Handler", menuName = "Handy 2D Tools/Management/Scenes/Scene Handler")]
    public class SceneHandler : ScriptableObject, IBootable
    {
        #region Inspector

        [Header("Debugging")]
        [Space]
        [SerializeField]
        private bool _debug;

        [Header("Handling Values")]
        [Space]
        [SerializeField]
        private float _maxUnloadTime;

        [SerializeField]
        private float _maxLoadTime;

        [Header("Curtains")]
        [Space]
        [SerializeField]
        protected CurtainsHandler _curtainsHandler;

        [Header("Fixed Scenes")]
        [Space]
        [SerializeField]
        protected SceneInfo _gameStartingScene;

        [SerializeField]
        protected SceneInfo _mainMenuScene;

        [Header("Transitions")]
        [Space]
        [SerializeField]
        protected GameObject _defaultSceneTransitionPrefab;

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<SceneInfo> FirstSceneEvent;

        [SerializeField]
        public UnityEvent<SceneInfo> SceneStartedEvent;

        [SerializeField]
        public UnityEvent<SceneInfo> SceneEndedEvent;

        #endregion

        #region  Fields

        // Load Progress
        protected float _currentSceneLoadProgress = 0;

        // Scenes
        protected SceneInfo _currentSceneInfo;

        // Scene Transition
        protected AsyncOperation _currentSceneLoadOperation;
        protected AsyncOperation _currentSceneUnloadOperation;

        //  Loading Subjects
        protected List<ISceneLoadSubject> _sceneLoadSubjects = new List<ISceneLoadSubject>();
        protected List<ISceneUnloadSubject> _sceneUnloadSubjects = new List<ISceneUnloadSubject>();

        // Status
        private SceneHandlerStatus _status = SceneHandlerStatus.Idle;

        #endregion

        #region Properties

        // Load Progress
        public float currentSceneLoadProgress { get { return _currentSceneLoadProgress; } set { _currentSceneLoadProgress = value; } }

        #endregion

        #region Getters

        public SceneInfo currentSceneInfo => _currentSceneInfo;

        public CurtainsHandler curtainsHandler => _curtainsHandler;

        // Scene Transition
        public GameObject defaultSceneTransitionPrefab => _defaultSceneTransitionPrefab;

        #endregion


        #region Bootable

        public Task BootableBoot()
        {
            _currentSceneInfo = null;
            _status = SceneHandlerStatus.Idle;

            return Task.CompletedTask;
        }

        public Task BootableDismiss()
        {
            return Task.CompletedTask;
        }

        #endregion


        #region Scene Management

        public async Task<bool> LoadScene(SceneInfo targetSceneInfo, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool closeCurtains = true, Func<Task> BeforeOpenCurtainsTask = null)
        {
            if (_status != SceneHandlerStatus.Idle)
            {
                if (_debug)
                    Debug.LogWarning($"{name} - {GetType().Name} - Tryed loading {targetSceneInfo.sceneField} while handler is Idle.");

                return false;
            }

            if (targetSceneInfo == null)
            {
                if (_debug)
                    Debug.LogWarning($"{name} - {GetType().Name} - Attempting to load an empty scene.");
                return false;
            }

            _status = SceneHandlerStatus.Busy;

            SceneEndedEvent.Invoke(_currentSceneInfo);

            if (closeCurtains)
                await CloseCurtains(_currentSceneInfo);

            await UnloadCurrentScene();

            _currentSceneInfo = targetSceneInfo;

            await LoadCurrentScene(loadSceneMode);

            if (BeforeOpenCurtainsTask != null)
                await BeforeOpenCurtainsTask();

            await OpenCurtains();

            _status = SceneHandlerStatus.Idle;

            SceneStartedEvent.Invoke(_currentSceneInfo);

            return true;
        }

        public async Task LoadMainMenu()
        {
            await LoadScene(_mainMenuScene);
        }

        #endregion

        #region Curtains        

        protected async Task CloseCurtains(SceneInfo sceneInfo)
        {
            GameObject transitionPrefab = sceneInfo.enterTransitionPrefab != null ? sceneInfo.enterTransitionPrefab : _defaultSceneTransitionPrefab;
            await _curtainsHandler.CloseCurtains(transitionPrefab);
        }

        protected async Task OpenCurtains()
        {
            await _curtainsHandler.OpenCurtains();
        }

        #endregion

        #region Loading Process

        protected async Task LoadCurrentScene(LoadSceneMode loadSceneMode)
        {
            _currentSceneLoadOperation = SceneManager.LoadSceneAsync(_currentSceneInfo.sceneField, loadSceneMode); // Starts loading the scene

            await TrackLoadProgress();
            await PerformLoadSubjectsTasks(_currentSceneInfo);
        }

        protected async Task TrackLoadProgress()
        {
            _currentSceneLoadProgress = 0;

            float elapsedTime = 0;

            while (!_currentSceneLoadOperation.isDone && elapsedTime <= _maxLoadTime)
            {
                _currentSceneLoadProgress = _currentSceneLoadOperation.progress;
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            _currentSceneLoadProgress = 100;

            if (!_currentSceneLoadOperation.isDone)
            {
                if (_debug)
                    Debug.LogWarning($"{name} - {GetType().Name} - Loading {_currentSceneInfo.sceneField} scene took more than {_maxLoadTime}. Forcing system to move on.");
            }
        }

        protected async Task PerformLoadSubjectsTasks(SceneInfo sceneInfo)
        {
            List<Task> sceneLoadSubjectTasks = new List<Task>();

            foreach (ISceneLoadSubject sceneLoadSubject in _sceneLoadSubjects)
            {
                sceneLoadSubjectTasks.Add(sceneLoadSubject.SceneLoadingTask(sceneInfo));
            }

            await Task.WhenAll(sceneLoadSubjectTasks);
        }

        public void RegisterSceneLoadSubject(ISceneLoadSubject loadSubject)
        {
            if (!_sceneLoadSubjects.Contains(loadSubject))
            {
                _sceneLoadSubjects.Add(loadSubject);
            }
        }

        public void UnregisterSceneLoadSubject(ISceneLoadSubject loadSubject)
        {
            _sceneLoadSubjects.Remove(loadSubject);
        }

        #endregion

        #region Unloading process

        public async Task UnloadCurrentScene()
        {
            if (_currentSceneInfo == null) return;

            await PerformUnloadSubjectsTasks(_currentSceneInfo); // subjects must perform before actual unload

            _currentSceneUnloadOperation = SceneManager.UnloadSceneAsync(_currentSceneInfo.sceneField); // Starts unloading the scene

            await TrackUnloadProgress();
        }

        private async Task TrackUnloadProgress()
        {

            float elapsedTime = 0;

            while (!_currentSceneUnloadOperation.isDone && elapsedTime <= _maxUnloadTime)
            {
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            if (!_currentSceneUnloadOperation.isDone)
            {
                if (_debug)
                    Debug.LogWarning($"{name} - {GetType().Name} - Unloading {_currentSceneInfo.sceneField} scene took more than {_maxUnloadTime}. Forcing system to move on.");
            }
        }

        protected async Task PerformUnloadSubjectsTasks(SceneInfo sceneInfo)
        {
            List<Task> sceneUnloadSubjectTasks = new List<Task>();

            foreach (ISceneLoadSubject sceneUnloadSubject in _sceneUnloadSubjects)
            {
                sceneUnloadSubjectTasks.Add(sceneUnloadSubject.SceneLoadingTask(sceneInfo));
            }

            await Task.WhenAll(sceneUnloadSubjectTasks);
        }

        public void RegisterSceneUnloadSubject(ISceneUnloadSubject unloadSubject)
        {
            if (!_sceneUnloadSubjects.Contains(unloadSubject))
            {
                _sceneUnloadSubjects.Add(unloadSubject);
            }
        }

        public void UnregisterSceneUnloadSubject(ISceneUnloadSubject unloadSubject)
        {
            _sceneUnloadSubjects.Remove(unloadSubject);
        }

        #endregion
    }

    public enum SceneHandlerStatus
    {
        Idle,
        Busy,
    }
}
