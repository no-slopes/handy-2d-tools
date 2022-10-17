using System.Collections;
using System.Collections.Generic;
using H2DT.Management.Graphics;
using TMPro;
using UnityEngine;

namespace H2DT.UI.Prefs.Graphics
{
    [RequireComponent(typeof(TMP_Dropdown))]
    [AddComponentMenu("Handy 2D Tools/UI/Prefs/Graphics/Quality Dropdown")]
    public class UIGraphicsQualityDropdown : HandyComponent
    {

        #region Inspector

        [Header("Configuration")]
        [Space]
        [SerializeField]
        protected GraphicsHandler _handler;

        [Header("Needed Objects")]
        [Space]
        [SerializeField]
        protected TMP_Dropdown _qualityDropdown;

        #endregion


        #region Mono

        protected void Awake()
        {
            if (_qualityDropdown == null)
                FindComponent<TMP_Dropdown>(ref _qualityDropdown);

            StartQualityDropdown();
        }

        protected void OnEnable()
        {
            _qualityDropdown.onValueChanged.AddListener(OnQualityValueChange);
        }

        protected void OnDisable()
        {
            _qualityDropdown.onValueChanged.RemoveListener(OnQualityValueChange);
        }

        #endregion

        #region  Logic

        protected void StartQualityDropdown()
        {
            _qualityDropdown.ClearOptions();

            List<string> options = new List<string>();

            foreach (string qualityName in _handler.qualitySettings)
            {
                options.Add(qualityName);
            }

            _qualityDropdown.AddOptions(options);
            _qualityDropdown.SetValueWithoutNotify(_handler.currentQualityIndex);
        }

        #endregion


        #region UI Callbacks

        protected void OnQualityValueChange(int index)
        {
            _handler.SetQualityLevel(index);
        }

        #endregion

    }
}
