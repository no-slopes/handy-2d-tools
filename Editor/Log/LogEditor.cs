using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using H2DT.Debugging;

namespace H2DT.Editor
{
    public class LogEditor
    {

        [MenuItem("GameObject/Handy 2D Tools/Logger/Log")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject logger = new GameObject(typeof(Log).Name);
            logger.AddComponent<Log>();
            GameObjectUtility.SetParentAndAlign(logger, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(logger, "Create " + logger.name);
            Selection.activeObject = logger;
        }

    }

}