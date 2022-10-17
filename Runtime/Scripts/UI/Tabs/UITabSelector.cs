using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using H2DT;
using H2DT.Debugging;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HD2T.UI.Tabs
{
    public abstract class UITabSelector : HandyComponent
    {
        #region Inspector

        [Header("Tab Selector")]
        [Space]
        [SerializeField]
        protected UITabGroup _group;

        [SerializeField]
        protected UITabPage _page;

        #endregion

        #region Fields

        protected bool _interactable;
        protected bool _selected;

        #endregion

        #region  Properties

        public bool interactable => _interactable;
        public bool selected => _selected;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            if (_group == null) FindComponentInParent<UITabGroup>(ref _group);
            if (_page == null) Log.Danger($"{gameObject.name}: No page set for {GetType().Name}");
        }

        protected virtual void OnEnable()
        {
            _group.RegisterSelector(this);
        }

        protected virtual void OnDisable()
        {
            _group.UnregisterSelector(this);
        }

        #endregion

        #region  Logic

        public void ToggleInteractions()
        {
            if (_interactable)
            {
                DisableInteractions();
            }
            else
            {
                EnableInteractions();
            }
        }

        public void EnableInteractions()
        {
            _interactable = true;
        }

        public void DisableInteractions()
        {
            _interactable = false;
        }

        #endregion

        #region Logic

        public virtual void SelectSilently()
        {
            _page.ActivateSilently();
        }

        /// <summary>
        /// Transitions into tab selected
        /// </summary>
        public async Task Select()
        {
            _selected = true;
            await _page.Activate();
        }

        /// <summary>
        /// Transitions into tab selected
        /// </summary>
        public async Task Unselect()
        {
            _selected = false;
            await _page.Deactivate();
        }


        #endregion

        #region Abstractions


        #endregion
    }
}
