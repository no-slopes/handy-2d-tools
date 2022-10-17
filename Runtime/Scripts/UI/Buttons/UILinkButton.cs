using UnityEngine;
using H2DT.Debugging;
using UnityEditor;
using UnityEngine.UI;

namespace H2DT.UI.Buttons
{
    public class UILinkButton : UIButton
    {
        #region Fields

        [Space]
        [SerializeField]
        protected string _externalLink;

        #endregion

        #region  Getters

        public string externalLink => _externalLink;

        #endregion

        #region Mono

        protected override void Awake()
        {
            base.Awake();

            if (string.IsNullOrEmpty(_externalLink))
            {
                Log.Danger($"{gameObject.name} - Empty link detected");
            }
        }

        protected override void OnButtonClick()
        {
            Application.OpenURL(_externalLink);
        }

        #endregion
    }
}
