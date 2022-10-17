using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT;
using H2DT.Management.Scenes;
using UnityEngine;
using UnityEngine.Events;

namespace H2DT.Management.Curtains
{
    [CreateAssetMenu(fileName = "Curtains Handler", menuName = "Handy 2D Tools/Management/Curtains/Curtains Handler")]
    public class CurtainsHandler : ScriptableObject
    {
        #region Fields

        protected SceneTransition _currentTransition;

        public UnityEvent<SceneTransition> ClosedCurtainsEvent = new UnityEvent<SceneTransition>();
        public UnityEvent<SceneTransition> OpenedCurtainsEvent = new UnityEvent<SceneTransition>();

        #endregion

        #region Properties

        public bool transitioning => _currentTransition != null;
        public SceneTransition currentTransition => _currentTransition;

        #endregion

        #region Logic

        public async Task CloseCurtains(GameObject transitionPrefab)
        {
            if (transitionPrefab == null) return;

            GameObject transitionObject = Instantiate(transitionPrefab);
            _currentTransition = transitionObject.GetComponent<SceneTransition>();

            if (_currentTransition == null) return;

            await _currentTransition.CloseCurtains();
            ClosedCurtainsEvent.Invoke(_currentTransition);
        }

        public async Task OpenCurtains()
        {
            if (!transitioning) return;

            await _currentTransition.OpenCurtains();
            OpenedCurtainsEvent.Invoke(_currentTransition);
            Destroy(_currentTransition.gameObject);
            _currentTransition = null;
        }

        #endregion       
    }
}
