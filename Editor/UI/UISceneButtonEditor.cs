using UnityEngine;
using UnityEditor;
using H2DT.UI.Buttons;

namespace H2DT.Editor
{
    public class UISceneButtonEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/UI/Buttons/Scene")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject buttonObject = new GameObject("Scene Button");
            buttonObject.AddComponent<UISceneButton>();
            GameObjectUtility.SetParentAndAlign(buttonObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(buttonObject, "Create " + buttonObject.name);
            Selection.activeObject = buttonObject;
        }
    }
}