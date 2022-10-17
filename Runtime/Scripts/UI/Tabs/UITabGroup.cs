using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2DT;
using H2DT.Debugging;
using UnityEngine;

namespace HD2T.UI.Tabs
{
    public class UITabGroup : HandyComponent
    {
        #region Inspector
        [Header("Selectors")]
        [Space]
        [SerializeField]
        protected UITabButtonSelector _defaultSelector;

        #endregion

        #region Fields

        protected List<UITabSelector> _selectors = new List<UITabSelector>();
        protected UITabSelector _currentSelector;
        protected bool _busy;

        #endregion

        #region Mono

        protected void Awake()
        {
            Initialize();
        }

        protected void OnEnable()
        {
        }

        protected void OnDisable()
        {

        }

        #endregion

        #region Logic 

        private void Initialize()
        {
            if (_defaultSelector == null)
            {
                Log.Danger($"{gameObject.name} - {GetType().Name} - No default selector provided");
                return;
            }

            _currentSelector = _defaultSelector;
            _currentSelector.SelectSilently();
        }

        public async Task SelectTab(UITabSelector selector)
        {
            if (_busy || selector == null || selector == _currentSelector) { return; } // Won't execute if busy or selector is null

            _busy = true; // will only execute new changes after Change is complete

            await ChangeSelector(selector);

            _busy = false; // OK, tab can be changed again.
        }

        public void HilightTabSelector()
        {

        }

        public void RegisterSelector(UITabSelector selector)
        {
            if (_selectors.Contains(selector)) return;
            _selectors.Add(selector);
        }

        public void UnregisterSelector(UITabSelector selector)
        {
            _selectors.Remove(selector);
        }

        public async Task ChangeSelector(UITabSelector selector)
        {

            await _currentSelector?.Unselect();

            _currentSelector = selector;

            await _currentSelector.Select();
        }

        #endregion
    }
}
