using H2DT.SpriteAnimations;
using UnityEditor;
using UnityEngine;

namespace H2DT.SpriteAnimations.Editor
{
    // [CustomPropertyDrawer(typeof(SpriteAnimationCycle))]
    // public class SpriteAnimationCycleDrawer : PropertyDrawer
    // {

    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         //base.OnGUI(position, property, label);
    //         EditorGUI.BeginProperty(position, label, property);

    //         position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //         int indent = EditorGUI.indentLevel;
    //         EditorGUI.indentLevel = 0;

    //         Rect framesRect = new Rect(position.x, position.y, position.width, position.height);

    //         SerializedProperty framesProp = property.FindPropertyRelative("_frames");

    //         EditorGUI.PropertyField(framesRect, framesProp, GUIContent.none);

    //         EditorGUI.EndProperty();
    //     }

    //     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //     {
    //         return base.GetPropertyHeight(property, label);
    //     }
    // }
}