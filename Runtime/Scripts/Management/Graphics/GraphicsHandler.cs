using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Debugging;
using H2DT.Management.Booting;
using UnityEngine;
using static H2DT.Utils.Structs;

namespace H2DT.Management.Graphics
{
    [CreateAssetMenu(fileName = "Graphic Handler", menuName = "Handy 2D Tools/Management/Graphics/Graphic Handler")]
    public class GraphicsHandler : ScriptableObject, IBootable
    {
        #region Static Strings

        public static string GraphicsManagerPrefPrefix = "HD2T_Graphics_";
        protected static string QualityPrefName = "Quality";
        protected static string ResolutionWidthPrefName = "ResolutionWidth";
        protected static string ResolutionHeightPrefName = "ResolutionHeight";
        protected static string FullScreenPrefName = "FullScreen";

        #endregion

        #region Fields

        protected List<ScreenResolution> _resolutions = new List<ScreenResolution>();
        protected ScreenResolution _currentScreenResolution;

        protected string[] _qualitySettings;
        protected int _currentQualityIndex;

        #endregion

        #region Getters

        public bool isFullScreen => Screen.fullScreen;

        public List<ScreenResolution> resolutions => _resolutions;
        public ScreenResolution currentScreenResolution => _currentScreenResolution;

        public string[] qualitySettings => _qualitySettings;
        public int currentQualityIndex => _currentQualityIndex;

        #endregion

        #region Mono

        public Task BootableBoot()
        {
            Resolution[] resolutions = Screen.resolutions;

            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                Resolution target = resolutions[i];
                ScreenResolution existent = _resolutions.Find(r => r.width == target.width && r.height == target.height);

                if (StructIsNull<ScreenResolution>(existent))
                {
                    _resolutions.Add(new ScreenResolution(target.width, target.height));
                }
            }

            _qualitySettings = QualitySettings.names;

            SetInitialQuality();
            SetInitialResolution();
            SetInitialFullScreen();

            return Task.CompletedTask;
        }

        public Task BootableDismiss()
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Logic

        public void SetFullScreen(bool fullScreen)
        {
            Screen.fullScreen = fullScreen;
            PlayerPrefs.SetInt(GraphicsManagerPrefPrefix + FullScreenPrefName, fullScreen ? 1 : 0);
        }

        public void SetResolution(int resolutionIndex)
        {
            ScreenResolution screenResolution = _resolutions[resolutionIndex];

            if (StructIsNull<ScreenResolution>(screenResolution))
            {
                Log.Danger($"Attempting to change into inexistent resolution using {resolutionIndex} as index.");
                return;
            }

            _currentScreenResolution = screenResolution;

            PlayerPrefs.SetInt(GraphicsManagerPrefPrefix + ResolutionWidthPrefName, _currentScreenResolution.width);
            PlayerPrefs.SetInt(GraphicsManagerPrefPrefix + ResolutionHeightPrefName, _currentScreenResolution.height);

            Screen.SetResolution(screenResolution.width, screenResolution.height, Screen.fullScreen);
        }


        public void SetQualityLevel(int qualityIndex)
        {
            if (qualityIndex > _qualitySettings.Length - 1)
            {
                Log.Danger($"Attempting to change into inexistent quality level using {qualityIndex} as index.");
                return;
            }

            _currentQualityIndex = qualityIndex;

            PlayerPrefs.SetInt(GraphicsManagerPrefPrefix + QualityPrefName, _currentQualityIndex);

            QualitySettings.SetQualityLevel(qualityIndex);
        }

        protected void SetInitialFullScreen()
        {
            if (PlayerPrefs.HasKey(GraphicsManagerPrefPrefix + FullScreenPrefName))
            {
                bool fullScreen = PlayerPrefs.GetInt(GraphicsManagerPrefPrefix + FullScreenPrefName) != 0 ? true : false;
                SetFullScreen(fullScreen);
                return;
            }

            SetFullScreen(true);
        }

        protected void SetInitialResolution()
        {
            // If player saved preferences
            if (PlayerPrefs.HasKey(GraphicsManagerPrefPrefix + ResolutionWidthPrefName) && PlayerPrefs.HasKey(GraphicsManagerPrefPrefix + ResolutionHeightPrefName))
            {
                // Load width and height from prefs
                int width = PlayerPrefs.GetInt(GraphicsManagerPrefPrefix + ResolutionWidthPrefName);
                int height = PlayerPrefs.GetInt(GraphicsManagerPrefPrefix + ResolutionHeightPrefName);

                // Finds on the _resolutions list
                int index = _resolutions.FindIndex(r => r.width == width && r.height == height);

                // If found, set as _currentScreenResolution 
                if (index >= 0)
                {
                    SetResolution(index);
                    return;
                }
            }

            SetResolution(0); // Sets for the first resolution at the list
        }

        protected void SetInitialQuality()
        {
            int qualityLevelIndex = QualitySettings.GetQualityLevel();

            // If player saved preferences
            if (PlayerPrefs.HasKey(GraphicsManagerPrefPrefix + QualityPrefName))
            {
                int prefIndex = PlayerPrefs.GetInt(GraphicsManagerPrefPrefix + QualityPrefName);

                if (prefIndex != qualityLevelIndex && prefIndex <= _qualitySettings.Length - 1)
                {
                    SetQualityLevel(prefIndex);
                    return;
                }
            }

            SetQualityLevel(qualityLevelIndex);
        }

        #endregion
    }
}
