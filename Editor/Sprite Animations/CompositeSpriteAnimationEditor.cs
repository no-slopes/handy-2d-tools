using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using H2DT.SpriteAnimations;

namespace H2DT.SpriteAnimations.Editor
{
    [CustomEditor(typeof(CompositeSpriteAnimation))]
    [CanEditMultipleObjects]
    public class CompositeSpriteAnimationEditor : SpriteAnimationEditor
    {
        protected SerializedProperty _hasAntecipation;
        protected SerializedProperty _antecipationCycle;
        protected SerializedProperty _coreCycle;
        protected SerializedProperty _loopableCore;
        protected SerializedProperty _hasRecovery;
        protected SerializedProperty _recoveryCycle;

        protected ReorderableList _antecipationFramesReorderableList;
        protected ReorderableList _coreFramesReorderableList;
        protected ReorderableList _recoveryFramesReorderableList;

        protected override void OnEnable()
        {
            base.OnEnable();

            _hasAntecipation = serializedObject.FindProperty("_hasAntecipation");
            _antecipationCycle = serializedObject.FindProperty("_antecipationCycle");
            var antecipationFrames = _antecipationCycle.FindPropertyRelative("_frames");

            _coreCycle = serializedObject.FindProperty("_coreCycle");
            _loopableCore = serializedObject.FindProperty("_loopableCore");
            var coreFrames = _coreCycle.FindPropertyRelative("_frames");

            _hasRecovery = serializedObject.FindProperty("_hasRecovery");
            _recoveryCycle = serializedObject.FindProperty("_recoveryCycle");
            var recoveryFrames = _recoveryCycle.FindPropertyRelative("_frames");

            _antecipationFramesReorderableList = new ReorderableList(serializedObject, antecipationFrames, true, true, true, true);
            _antecipationFramesReorderableList.onAddCallback = OnAddCallback;
            _antecipationFramesReorderableList.onRemoveCallback = OnRemoveCallback;
            _antecipationFramesReorderableList.onReorderCallback = OnReorderCallback;
            _antecipationFramesReorderableList.drawElementCallback = DrawAntecipationFramesListElements;
            _antecipationFramesReorderableList.drawHeaderCallback = DrawAntecipationFramesListHeader;

            _coreFramesReorderableList = new ReorderableList(serializedObject, coreFrames, true, true, true, true);
            _coreFramesReorderableList.onAddCallback = OnAddCallback;
            _coreFramesReorderableList.onRemoveCallback = OnRemoveCallback;
            _coreFramesReorderableList.onReorderCallback = OnReorderCallback;
            _coreFramesReorderableList.drawElementCallback = DrawCoreFramesListElements;
            _coreFramesReorderableList.drawHeaderCallback = DrawCoreFramesListHeader;

            _recoveryFramesReorderableList = new ReorderableList(serializedObject, recoveryFrames, true, true, true, true);
            _recoveryFramesReorderableList.onAddCallback = OnAddCallback;
            _recoveryFramesReorderableList.onRemoveCallback = OnRemoveCallback;
            _recoveryFramesReorderableList.onReorderCallback = OnReorderCallback;
            _recoveryFramesReorderableList.drawElementCallback = DrawRecoveryFramesListElements;
            _recoveryFramesReorderableList.drawHeaderCallback = DrawRecoveryFramesListHeader;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_hasAntecipation);

            if (_hasAntecipation.boolValue)
            {
                EditorGUILayout.Space();
                _antecipationFramesReorderableList.DoLayoutList();
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_loopableCore);

            EditorGUILayout.Space();
            _coreFramesReorderableList.DoLayoutList();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_hasRecovery);

            if (_hasRecovery.boolValue)
            {
                EditorGUILayout.Space();
                _recoveryFramesReorderableList.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawAntecipationFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _antecipationFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawAntecipationFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Antecipation Cycle");
        }

        protected void DrawCoreFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _coreFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawCoreFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Core Cycle");
        }

        protected void DrawRecoveryFramesListElements(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _recoveryFramesReorderableList.serializedProperty.GetArrayElementAtIndex(index); // The element in the list
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
        }

        protected void DrawRecoveryFramesListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Recovery Cycle");
        }

        protected void OnAddCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize++;
            SerializedProperty addedElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
            addedElement.FindPropertyRelative("_sprite").objectReferenceValue = null;
            addedElement.FindPropertyRelative("_name").stringValue = null;
            ReorderIds();
        }

        protected void OnRemoveCallback(ReorderableList list)
        {
            list.serializedProperty.arraySize--;
            ReorderIds();
        }

        protected void OnReorderCallback(ReorderableList list)
        {
            ReorderIds();
        }

        protected void ReorderIds()
        {
            SerializedProperty currentElement;

            int antecipationCount = _antecipationFramesReorderableList.serializedProperty.arraySize;
            int coreCount = _coreFramesReorderableList.serializedProperty.arraySize + antecipationCount;
            int recoveryCount = _recoveryFramesReorderableList.serializedProperty.arraySize + coreCount;

            for (int i = 0; i < antecipationCount; i++)
            {
                currentElement = _antecipationFramesReorderableList.serializedProperty.GetArrayElementAtIndex(i);
                currentElement.FindPropertyRelative("_id").intValue = i + 1;
            }

            for (int i = 0; i < coreCount - antecipationCount; i++)
            {
                currentElement = _coreFramesReorderableList.serializedProperty.GetArrayElementAtIndex(i);
                currentElement.FindPropertyRelative("_id").intValue = antecipationCount + i + 1;
            }

            for (int i = 0; i < recoveryCount - coreCount; i++)
            {
                currentElement = _recoveryFramesReorderableList.serializedProperty.GetArrayElementAtIndex(i);
                currentElement.FindPropertyRelative("_id").intValue = coreCount + i + 1;
            }

        }

    }
}