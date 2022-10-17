using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HD2T.UI.Tabs
{
    public class UITabImageSelector : UITabSelector, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {

        public void Highlight()
        {

        }

        public void Rest()
        {

        }

        #region Logic

        #endregion

        #region Event Systems Callbacks

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable) return;
            _group.SelectTab(this).Wait();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_interactable) return;
            Highlight();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_interactable) return;
            Rest();
        }

        #endregion

    }
}
