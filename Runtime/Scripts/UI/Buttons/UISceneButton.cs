using System.Collections;
using System.Collections.Generic;
using H2DT.Management;
using H2DT.Management.Scenes;
using UnityEngine;
using H2DT.Debugging;
using UnityEditor;

namespace H2DT.UI.Buttons
{
    public class UISceneButton : UIButton
    {

        #region Fields

        [Space]
        [SerializeField]
        private SceneHandler _sceneHandler;

        [Space]
        [SerializeField]
        protected SceneInfo _sceneInfo;

        #endregion

        #region  Getters

        public SceneInfo sceneInfo => _sceneInfo;

        #endregion

        #region  Mono

        protected override void Awake()
        {
            base.Awake();

            if (_sceneInfo == null)
            {
                Log.Danger($"{gameObject.name} - Null scene info");
            }
        }

        #endregion

        #region Logic

        protected override async void OnButtonClick()
        {
            await _sceneHandler.LoadScene(_sceneInfo);
        }

        #endregion
    }
}
