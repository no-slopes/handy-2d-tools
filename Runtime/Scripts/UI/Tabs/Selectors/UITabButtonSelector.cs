using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2DT;
using H2DT.Debugging;
using H2DT.Generics.Transitions;
using UnityEngine;
using UnityEngine.UI;

namespace HD2T.UI.Tabs
{
    [RequireComponent(typeof(Button))]
    public class UITabButtonSelector : UITabSelector
    {
        #region Inspector

        [Header("Tab Button Selector")]
        [Space]
        [SerializeField]
        protected Button _button;

        #endregion

        #region Fields

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            if (_button == null) FindComponent<Button>(ref _button);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _button.onClick.AddListener(OnButtonClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _button.onClick.RemoveListener(OnButtonClick);
        }

        #endregion

        #region Logic

        public override void SelectSilently()
        {
            base.SelectSilently();
            _button.Select();
        }

        #endregion

        #region Button Callbacks

        protected void OnButtonClick()
        {
            _group.SelectTab(this).Wait();
        }

        #endregion
    }
}
