using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace H2DT.UI.Dialogs
{
    public class UIModal : HandyComponent
    {
        #region Inspector        

        [Header("Skeleton")]
        [Space]
        [SerializeField]
        protected bool _headerOn;

        [SerializeField]
        protected bool _footerOn;

        [Header("Skeleton")]
        [Space]
        [SerializeField]
        protected Transform _header;

        [SerializeField]
        protected Transform _body;

        [SerializeField]
        protected Transform _footer;

        [Header("Media")]
        [Space]
        [SerializeField]
        protected TextMeshProUGUI _titleText;

        [SerializeField]
        protected Image _mainImage;

        [SerializeField]
        protected TextMeshProUGUI _mainText;

        [Header("Buttons")]
        [Space]
        [SerializeField]
        protected Button _closeButton;

        [SerializeField]
        protected Button _confirmButton;

        [SerializeField]
        protected Button _cancelButton;

        [SerializeField]
        protected Button _alternatoveButton;

        #endregion
    }
}
