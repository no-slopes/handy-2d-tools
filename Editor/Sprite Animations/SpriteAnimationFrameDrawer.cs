using H2DT.SpriteAnimations;
using UnityEditor;
using UnityEngine;

namespace H2DT.SpriteAnimations.Editor
{
    [CustomPropertyDrawer(typeof(SpriteAnimationFrame))]
    public class SpriteAnimationFrameDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SimpleSpriteAnimation animation = property.serializedObject.targetObject as SimpleSpriteAnimation;

            Rect idRect = new Rect(position.x, position.y, position.width * 0.1f, position.height);
            Rect spriteRect = new Rect(position.x + position.width * 0.11f, position.y, position.width * 0.49f, position.height);
            Rect nameRect = new Rect(position.x + position.width * 0.62f, position.y, position.width * 0.38f, position.height);

            SerializedProperty idProp = property.FindPropertyRelative("_id");
            SerializedProperty spriteProp = property.FindPropertyRelative("_sprite");
            SerializedProperty nameProp = property.FindPropertyRelative("_name");

            GUI.enabled = false;
            EditorGUI.PropertyField(idRect, idProp, GUIContent.none);
            GUI.enabled = true;
            EditorGUI.PropertyField(spriteRect, spriteProp, GUIContent.none);
            EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}