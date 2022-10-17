using System;
using System.Threading.Tasks;
using H2DT.Debugging;
using H2DT.Management.Booting;
using H2DT.Management.Scenes;
using H2DT.NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace H2DT.Management.Levels
{
    [CreateAssetMenu(fileName = "Level Handler", menuName = "Handy 2D Tools/Management/Levels/Level Handler")]
    public class LevelHandler : ScriptableObject, IBootable
    {
        #region Inspector

        [Header("Scenes")]
        [Space]
        [SerializeField]
        private SceneHandler _sceneHandler;

        [Header("Levels")]
        [Space]
        [SerializeField]
        private LevelInfo _anchorLevel;

        [Header("Events")]
        [Space]
        [SerializeField]
        public UnityEvent<Level> LevelLoadedEvent;

        [SerializeField]
        public UnityEvent<Level> LevelEndedEvent;

        #endregion

        #region Fields

        [Foldout("Debug")]
        [ReadOnly]
        [SerializeField]
        protected Level _currentLevel;

        [Foldout("Debug")]
        [ReadOnly]
        [SerializeField]
        protected object _previousLevelTrail;

        #endregion

        #region  Getters

        public Level currentLevel => _currentLevel;

        #endregion

        #region Booting

        public async Task BootableBoot()
        {
            await Task.Run(Clear);
        }

        public Task BootableDismiss()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Loading Logic

        public async Task<T> LoadLevel<T>(LevelInfo levelInfo, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool shouldCloseCurtains = true) where T : Level
        {
            if (!ValidateLevelInfo(levelInfo)) return null;

            bool loadResult = await _sceneHandler.LoadScene(levelInfo.scene, loadSceneMode, shouldCloseCurtains, () => BeforeSceneStart(levelInfo));

            if (!loadResult) return null;

            LevelLoadedEvent.Invoke(_currentLevel);

            return _currentLevel as T;
        }

        private bool ValidateLevelInfo(LevelInfo levelInfo)
        {
            return levelInfo != null && levelInfo.scene != null && !string.IsNullOrEmpty(levelInfo.scene.sceneField);
        }

        public async Task BeforeSceneStart(LevelInfo levelInfo)
        {
            if (levelInfo.mustInstantiate)
            {
                await InstantiateAndInitialize(levelInfo);
            }
            else
            {
                await FindAndInitialize();
            }

            if (_currentLevel == null && UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError($"{name} - {GetType().Name} - Missing level on loaded scene.");
            }
        }

        protected async Task FindAndInitialize()
        {
            await FindLevel(TimeSpan.FromSeconds(0.2f), 5);

            await InitializeLevel(_currentLevel);
        }

        protected async Task InstantiateAndInitialize(LevelInfo levelInfo)
        {
            Vector3 levelPos = Vector3.zero;

            LevelReceptor receptor = FindObjectOfType<LevelReceptor>();

            if (receptor != null)
            {
                levelPos = receptor.transform.position;
                Destroy(receptor.gameObject);
            }

            if (levelInfo.levelPrefab == null)
            {
                Log.Danger($"{GetType().Name} - Level info missing prefab object for {levelInfo.levelName}.");
                return;
            }

            GameObject levelObject = Instantiate(levelInfo.levelPrefab, levelPos, Quaternion.identity);
            _currentLevel = levelObject.GetComponent<Level>();

            if (_currentLevel == null)
            {
                Log.Danger($"{GetType().Name} - Level info missing an object with an Level component.");
                Destroy(_currentLevel);
                return;
            }

            await InitializeLevel(_currentLevel);
        }

        protected async Task InitializeLevel(Level level)
        {
            if (level == null) return;

            if (!level.shouldSelfInitialize)
                await level.Initialize();
        }

        private async Task FindLevel(TimeSpan retryInterval, int maxAttemptCount = 3)
        {
            for (int attempted = 0; attempted < maxAttemptCount; attempted++)
            {
                if (attempted > 0)
                {
                    await Task.Delay(retryInterval);
                }

                _currentLevel = FindObjectOfType<Level>();

                if (_currentLevel != null)
                {
                    return;
                }
            }
        }

        #region Ending Level

        public async Task Leave(object trail = null)
        {
            _previousLevelTrail = trail;
            await EndLevel();
        }

        public async Task Leave(LevelInfo levelInfo, object trail = null)
        {
            _previousLevelTrail = trail;
            await EndLevel(levelInfo);
        }

        private Task EndLevel()
        {
            LevelEndedEvent.Invoke(_currentLevel);
            _currentLevel = null;

            return Task.CompletedTask;
        }

        private async Task EndLevel(LevelInfo levelInfo)
        {
            await EndLevel();

            if (levelInfo != null)
            {
                await _sceneHandler.LoadScene(levelInfo.scene);
            }
            else if (_anchorLevel != null)
            {
                await _sceneHandler.LoadScene(_anchorLevel.scene);
            }
            else
            {
                await _sceneHandler.LoadMainMenu();
            }
        }

        #endregion

        #region Trail

        public T GetPreviousLevelTrail<T>()
        {
            return (T)_previousLevelTrail;
        }

        #endregion

        /// <summary>
        /// Clears information about being in a level.
        /// </summary>
        protected void Clear()
        {
            _currentLevel = null;
            _previousLevelTrail = null;
        }

        #endregion
    }
}
