using System.Collections;
using System.Collections.Generic;
using H2DT.Management.Graphics;
using UnityEngine;
using UnityEngine.UI;

namespace H2DT.UI.Prefs.Graphics
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu("Handy 2D Tools/UI/Prefs/Graphics/Fullscreen Toggle")]
    public class UIGraphicsFullscreenToggle : HandyComponent
    {

        #region Inspector

        [Header("Configuration")]
        [Space]
        [SerializeField]
        protected GraphicsHandler _handler;

        [Header("Needed Objects")]
        [Space]

        [SerializeField]
        protected Toggle _fullScreenToggle;

        #endregion

        #region  Properties

        #endregion

        #region Mono

        protected void Awake()
        {
            if (_fullScreenToggle == null)
                FindComponent<Toggle>(ref _fullScreenToggle);

            StartFullScreenToggle();
        }

        protected void OnEnable()
        {
            _fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggle);
        }

        protected void OnDisable()
        {
            _fullScreenToggle.onValueChanged.RemoveListener(OnFullScreenToggle);
        }

        #endregion

        #region  Logic

        protected void StartFullScreenToggle()
        {
            _fullScreenToggle.isOn = _handler.isFullScreen;
        }

        #endregion

        #region UI Callbacks

        protected void OnFullScreenToggle(bool toggle)
        {
            _handler.SetFullScreen(toggle);
        }

        #endregion
    }
}
