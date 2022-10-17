using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using H2DT.SpriteAnimations;
using System.Collections.Generic;

namespace H2DT.SpriteAnimations.Editor
{
    [CustomEditor(typeof(ComboSpriteAnimation))]
    [CanEditMultipleObjects]
    public class ComboSpriteAnimationEditor : SpriteAnimationEditor
    {
        protected ComboSpriteAnimation SelectedComboSpriteAnimation => SelectedSpriteAnimation as ComboSpriteAnimation;

        protected SerializedProperty _cyclesProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _cyclesProperty = serializedObject.FindProperty("_cycles");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // DrawDefaultInspector();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_cyclesProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}