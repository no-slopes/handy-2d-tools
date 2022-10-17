using UnityEngine;
using UnityEditor;
using H2DT.UI.Buttons;

namespace H2DT.Editor
{
    public class UILinkButtonEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/UI/Buttons/Link")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject buttonObject = new GameObject("Link Button");
            buttonObject.AddComponent<UILinkButton>();
            GameObjectUtility.SetParentAndAlign(buttonObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create " + buttonObject.name);
            Selection.activeObject = buttonObject;
        }
    }
}