using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using H2DT.SpriteAnimations;
using System.Collections.Generic;

namespace H2DT.SpriteAnimations.Editor
{
    [CustomEditor(typeof(SimpleSpriteAnimation))]
    [CanEditMultipleObjects]
    public class SimpleSpriteAnimationEditor : SpriteAnimationEditor
    {
        protected SimpleSpriteAnimation SelectedSimpleSpriteAnimation => SelectedSpriteAnimation as SimpleSpriteAnimation;

        protected SerializedProperty _loopProperty;
        protected SerializedProperty _cycleProperty;

        protected ReorderableList _cycleReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();

            _loopProperty = serializedObject.FindProperty("_loop");
            _cycleProperty = serializedObject.FindProperty("_cycle");

            var cycleFrames = _cycleProperty.FindPropertyRelative("_frames");

            _cycleReorderableList = new ReorderableList(serializedObject, cycleFrames, true, true, true, true);

            _cycleReorderableList.displayAdd = true;
            _cycleReorderableList.onAddCallback = OnAddCallback;
            _cycleReorderableList.onRemoveCallback = OnRemoveCallback;
            _cycleReorderableList.drawElementCallback = DrawElementCallback;
            _cycleReorderableList.drawHeaderCallback = DrawHeaderCallback;
            _cycleReorderableList.onReorderCallback = OnReorderCallback;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_loopProperty);

            EditorGUILayout.Space();
            _cycleReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }

        protected void OnAddCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize++;
            SerializedProperty addedElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
            addedElement.FindPropertyRelative("_sprite").objectReferenceValue = null;
            addedElement.FindPropertyRelative("_name").stringValue = null;
            OnReorderCallback(list);
        }

        protected void OnRemoveCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize--;
            OnReorderCallback(list);
        }

        protected void OnReorderCallback(ReorderableList list)
        {
            SerializedProperty currentElement;

            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                currentElement = list.serializedProperty.GetArrayElementAtIndex(i);
                currentElement.FindPropertyRelative("_id").intValue = i + 1;
            }
        }

        protected void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _cycleReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            Rect propRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propRect, element, GUIContent.none);
        }

        protected void DrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, "Cycle");
        }
    }
}