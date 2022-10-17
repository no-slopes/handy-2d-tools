using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace H2DT.Icons.Editor
{
    [CustomEditor(typeof(DocumentedComponent))]
    public class HandyComponentIconEditor : UnityEditor.Editor
    {
        DocumentedComponent handyComponent => target as DocumentedComponent;

        private void SetIcons()
        {
            // this sets the icon on the game object containing our behaviour
            handyComponent.gameObject.SetIcon("Example.Editor.Resources.Icon.png");

            // this sets the icon on the script (which normally shows the blank page icon)
            MonoScript.FromMonoBehaviour(handyComponent).SetIcon("Example.Editor.Resources.Icon.png");
        }

        void Awake()
        {
            SetIcons();
        }

        public override void OnInspectorGUI()
        {
            Debug.Log("Teste");
            serializedObject.Update();
        }
    }
}