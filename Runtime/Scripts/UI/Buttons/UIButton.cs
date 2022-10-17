using UnityEngine;
using UnityEngine.UI;

namespace H2DT.UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public abstract class UIButton : HandyComponent
    {

        #region Fields

        protected Button _button;

        #endregion

        #region  Getters

        public Button button => _button;

        #endregion

        #region Mono

        protected virtual void Awake()
        {
            FindComponent<Button>(ref _button);
        }

        protected void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        protected void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        #endregion

        #region Abstratctions

        protected abstract void OnButtonClick();

        #endregion
    }
}
