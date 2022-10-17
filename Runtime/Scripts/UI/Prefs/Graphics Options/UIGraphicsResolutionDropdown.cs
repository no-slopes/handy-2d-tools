using System.Collections;
using System.Collections.Generic;
using H2DT.Management.Graphics;
using TMPro;
using UnityEngine;

namespace H2DT.UI.Prefs.Graphics
{
    [RequireComponent(typeof(TMP_Dropdown))]
    [AddComponentMenu("Handy 2D Tools/UI/Prefs/Graphics/Resolution Dropdown")]
    public class UIGraphicsResolutionDropdown : HandyComponent
    {

        #region Inspector

        [Header("Configuration")]
        [Space]
        [SerializeField]
        protected GraphicsHandler _handler;

        [Header("Needed Objects")]
        [Space]

        [SerializeField]
        protected TMP_Dropdown _resolutionDropdown;

        #endregion

        #region  Fields

        #endregion

        #region  Properties

        #endregion

        #region Mono

        protected void Awake()
        {
            if (_resolutionDropdown == null)
                FindComponent<TMP_Dropdown>(ref _resolutionDropdown);

            StartResolutionDropdown();
        }

        protected void OnEnable()
        {
            _resolutionDropdown.onValueChanged.AddListener(OnResolutionValueChange);
        }

        protected void OnDisable()
        {
            _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionValueChange);
        }

        #endregion

        #region  Logic

        protected void StartResolutionDropdown()
        {
            _resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int selectedIndex = 0;

            for (int i = 0; i < _handler.resolutions.Count; i++)
            {
                ScreenResolution screenResolution = _handler.resolutions[i];

                string optionText = screenResolution.width + " x " + screenResolution.height;

                options.Add(optionText);

                if (SameResolution(screenResolution, _handler.currentScreenResolution))
                {
                    selectedIndex = i;
                }
            }

            _resolutionDropdown.AddOptions(options);

            _resolutionDropdown.SetValueWithoutNotify(selectedIndex);
        }

        protected bool SameResolution(ScreenResolution res1, ScreenResolution res2)
        {
            return res1.width == res2.width && res1.height == res2.height;
        }

        #endregion

        #region UI Callbacks

        protected void OnResolutionValueChange(int index)
        {
            _handler.SetResolution(index);
        }

        #endregion
    }
}
