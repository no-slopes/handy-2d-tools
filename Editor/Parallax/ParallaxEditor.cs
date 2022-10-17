using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using H2DT.Environmental;

namespace H2DT.Editor
{

    public class ParallaxEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/Environmental/Parallax/Parallax")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject parallaxObject = new GameObject("Parallax");
            parallaxObject.AddComponent<Parallax>();
            GameObjectUtility.SetParentAndAlign(parallaxObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(parallaxObject, "Create " + parallaxObject.name);
            Selection.activeObject = parallaxObject;
        }
    }

    public class ParallaxLayerEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/Environmental/Parallax/ParallaxLayer")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject parallaxLayerObject = new GameObject("ParallaxLayer");
            parallaxLayerObject.AddComponent<SpriteRenderer>();
            parallaxLayerObject.AddComponent<ParallaxLayer>();
            GameObjectUtility.SetParentAndAlign(parallaxLayerObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(parallaxLayerObject, "Create " + parallaxLayerObject.name);
            Selection.activeObject = parallaxLayerObject;
        }
    }

    public class ParallaxTriggerEditor
    {
        [MenuItem("GameObject/Handy 2D Tools/Environmental/Parallax/ParallaxTrigger")]
        public static void CreateSeparator(MenuCommand menuCommand)
        {
            GameObject parallaxTriggerObject = new GameObject("ParallaxTrigger");
            parallaxTriggerObject.AddComponent<BoxCollider2D>();
            BoxCollider2D boxCollider2D = parallaxTriggerObject.GetComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            boxCollider2D.size = new Vector2(0.1f, 76f);
            parallaxTriggerObject.AddComponent<ParallaxTrigger>();
            GameObjectUtility.SetParentAndAlign(parallaxTriggerObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(parallaxTriggerObject, "Create " + parallaxTriggerObject.name);
            Selection.activeObject = parallaxTriggerObject;
        }
    }

}