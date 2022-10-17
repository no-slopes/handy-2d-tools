using System.Collections.Generic;
using H2DT.Management.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;
using H2DT.Debugging;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace H2DT.Management.Levels
{
    public class SubLevelAnchor : HandyComponent
    {

        #region  Inspector

        [SerializeField]
        protected Level _level;

        [SerializeField]
        protected float _targetDistance = 10;

        [SerializeField]
        protected SceneField _scene;

        #endregion

        #region Fields

        protected float _currentDistance;
        protected bool _loaded;

        protected List<GameObject> _targets = new List<GameObject>();

        protected AsyncOperation _sceneOperation;

        #endregion

        #region Properties

        #endregion

        #region Mono

        protected void Awake()
        {
            ValidadeLevelOrDestroy();
        }

        protected void Update()
        {
            Evaluate();
        }

        #endregion

        #region  Logic

        protected void ValidadeLevelOrDestroy()
        {
            if (_level != null) return;

            Log.Danger($"{gameObject.name} - Could not initialize since there is no reference for a level.");
            Destroy(gameObject);
        }

        protected void Evaluate()
        {
            if (_targets.Count == 0) return;
            if (_sceneOperation != null) return;

            foreach (GameObject target in _targets)
            {
                if (target == null) continue;

                _currentDistance = Vector2.Distance(transform.position, target.transform.position);

                bool inDistance = _currentDistance <= _targetDistance;

                if (inDistance && !_loaded)
                {
                    LoadScene();
                    return;
                }

                if (!inDistance && _loaded)
                {
                    UnloadScene();
                    return;
                }
            }
        }

        protected void LoadScene()
        {
            _sceneOperation = UnitySceneManager.LoadSceneAsync(_scene, LoadSceneMode.Additive);
            _sceneOperation.completed += OnLoadComplete;
        }

        protected void UnloadScene()
        {
            _sceneOperation = UnitySceneManager.UnloadSceneAsync(_scene);
            _sceneOperation.completed += OnUnloadComplete;
        }

        public void AddTarget(GameObject target)
        {
            if (!_targets.Contains(target))
            {
                _targets.Add(target);
            }
        }

        public void RemoveTarget(GameObject target)
        {
            _targets.Remove(target);
        }

        #endregion

        #region  Callbacks

        protected void OnLoadComplete(AsyncOperation operation)
        {
            _sceneOperation.completed -= OnLoadComplete;
            _loaded = true;
            _sceneOperation = null;
        }

        protected void OnUnloadComplete(AsyncOperation operation)
        {
            _sceneOperation.completed -= OnUnloadComplete;
            _loaded = false;
            _sceneOperation = null;
        }

        #endregion
    }
}
