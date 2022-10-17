using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT.Management.Booting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace H2DT.Management.Localization
{
    [CreateAssetMenu(fileName = "Localization Handler", menuName = "H2DT/Management/Localization/Localization Handler")]
    public class LocalizationHandler : SingleScriptaBootable<LocalizationHandler>
    {
        #region Inspector

        [Header("Localization Handler Configuration")]
        [Space]
        [SerializeField]
        private PlayerPrefLocaleSelector _prefLocaleSelector;

        [Header("Events")]
        [Space]
        [SerializeField]
        private UnityEvent<Locale> _localeChanged;

        #endregion

        #region Fields

        private Locale _currentLocale;
        private ILocalesProvider _localesProvider;

        #endregion

        #region Getters

        // Locales
        public Locale currentLocale => _currentLocale;
        public List<Locale> availableLocales => _localesProvider.Locales;

        // Events
        public UnityEvent<Locale> localeChanged => _localeChanged;

        #endregion

        #region Booting

        public override async Task BootableBoot()
        {
            await base.BootableBoot();
            await LocalizationSettings.InitializationOperation.Task;
            _localesProvider = LocalizationSettings.AvailableLocales;
            SetInitialLocale();
        }

        #endregion

        #region Logic        

        private void SetInitialLocale()
        {
            _currentLocale = _prefLocaleSelector.GetStartupLocale(_localesProvider);

            if (_currentLocale == null)
            {
                if (_localesProvider.Locales.Count > 0)
                {
                    _currentLocale = _localesProvider.Locales[0];
                }
                else
                {
                    Debug.LogError($"No locale found to initialize", this);
                }
            }

            SetLocale(_currentLocale.Identifier);
        }

        public void SetLocale(LocaleIdentifier localeIdentifier)
        {
            Locale locale = _localesProvider.GetLocale(localeIdentifier);
            SetLocale(locale);
        }

        public void SetLocale(Locale locale)
        {
            LocalizationSettings.SelectedLocale = locale;
            _currentLocale = LocalizationSettings.SelectedLocale;
            _localeChanged.Invoke(_currentLocale);
        }

        #endregion
    }
}