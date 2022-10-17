using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using H2DT.SpriteAnimations;
using System.Collections.Generic;
using System.Linq;

namespace H2DT.SpriteAnimations.Editor
{
    [CustomEditor(typeof(SpriteAnimation))]
    [CanEditMultipleObjects]
    public class SpriteAnimationEditor : UnityEditor.Editor
    {
        protected SpriteAnimation SelectedSpriteAnimation => target as SpriteAnimation;

        protected float timeTracker = 0;
        protected Sprite currentSprite;

        protected float currentCycleElapsedTime = 0.0f;
        protected SpriteAnimationFrame currentFrame;

        new protected SerializedProperty name;
        protected SerializedProperty fps;
        protected SerializedProperty animationCycleEnded;

        protected float FrameDuration => 1f / SelectedSpriteAnimation.FPS;
        protected float CurrentCycleDuration => FrameDuration * AllFrames.Count;
        protected int CurrentFrameIndex => Mathf.FloorToInt(currentCycleElapsedTime * AllFrames.Count / CurrentCycleDuration);

        protected bool HasAnimationAndFrames => SelectedSpriteAnimation != null && SelectedSpriteAnimation.AllFrames != null && SelectedSpriteAnimation.AllFrames.Count > 0;
        protected List<SpriteAnimationFrame> AllFrames => SelectedSpriteAnimation.AllFrames;

        protected virtual void OnEnable()
        {
            timeTracker = (float)EditorApplication.timeSinceStartup;

            name = serializedObject.FindProperty("_name");
            fps = serializedObject.FindProperty("_fps");

            EditorApplication.update += OnUpdate;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(name);
            EditorGUILayout.PropertyField(fps);

            serializedObject.ApplyModifiedProperties();
        }

        public override bool HasPreviewGUI()
        {
            return HasAnimationAndFrames;
        }

        public override bool RequiresConstantRepaint()
        {
            return HasAnimationAndFrames;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (currentSprite != null && currentSprite != null)
            {
                Texture t = currentSprite.texture;
                Rect tr = currentSprite.textureRect;
                Rect r2 = new Rect(tr.x / t.width, tr.y / t.height, tr.width / t.width, tr.height / t.height);

                Rect previewRect = r;

                float targetAspectRatio = tr.width / tr.height;
                float windowAspectRatio = r.width / r.height;
                float scaleHeight = windowAspectRatio / targetAspectRatio;

                if (scaleHeight < 1f)
                {
                    previewRect.width = r.width;
                    previewRect.height = scaleHeight * r.height;
                    previewRect.x = r.x;
                    previewRect.y = r.y + (r.height - previewRect.height) / 2f;
                }
                else
                {
                    float scaleWidth = 1f / scaleHeight;

                    previewRect.width = scaleWidth * r.width;
                    previewRect.height = r.height;
                    previewRect.x = r.x + (r.width - previewRect.width) / 2f;
                    previewRect.y = r.y;
                }

                GUI.DrawTextureWithTexCoords(previewRect, t, r2, true);
            }
        }

        protected void OnUpdate()
        {
            if (HasAnimationAndFrames)
            {
                float deltaTime = (float)EditorApplication.timeSinceStartup - timeTracker;
                timeTracker += deltaTime;
                currentSprite = EvaluateSprite(deltaTime);
            }
        }


        protected Sprite EvaluateSprite(float deltaTime)
        {
            currentCycleElapsedTime += deltaTime;

            HandleCycle();

            return currentFrame?.Sprite;
        }

        protected void HandleCycle()
        {

            if (currentCycleElapsedTime >= CurrentCycleDuration) // means cycle passed last frame
            {
                ResetCycle();
            }

            currentFrame = EvaluateCycleFrame();
        }

        protected SpriteAnimationFrame EvaluateCycleFrame()
        {
            if (currentFrame == null) return AllFrames[0];

            SpriteAnimationFrame evaluatedFrame = AllFrames.ElementAtOrDefault(CurrentFrameIndex);

            if (evaluatedFrame == null || evaluatedFrame == currentFrame) return currentFrame;

            return evaluatedFrame;
        }

        protected void ResetCycle()
        {
            currentCycleElapsedTime = 0f;
            currentFrame = null;
        }

    }
}