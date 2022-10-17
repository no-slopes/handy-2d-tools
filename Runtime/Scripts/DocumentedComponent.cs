using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using H2DT.NaughtyAttributes;
using System;
using H2DT.Debugging;
using static H2DT.Utils.DocUtils;

namespace H2DT
{
    public abstract class DocumentedComponent : HandyComponent
    {
        #region Inspector

        [Button, Tooltip("Opens this component's documentation webpage")]
        protected virtual void OpenDocs()
        {
            Application.OpenURL(Url + "/en/" + docPath);
        }

        // [Button, Tooltip("Abrir a página da documentação do componente")]
        // protected virtual void AbrirDocs()
        // {
        //     Application.OpenURL(Url + "/pt_BR/" + DocPath);
        // }

        #endregion

        protected abstract string docPath { get; }
    }
}
