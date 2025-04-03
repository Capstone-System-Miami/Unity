using UnityEngine;
using UnityEditor;
using System;

namespace SystemMiami.CustomEditor
{
    public class AnimationKeyframeLerper : EditorWindow
    {
        private AnimationClip selectedClip;
        private string targetPropertyName = "m_LocalPosition.x"; // Default property name
        private float duration = 1f; // Default duration in seconds
        private int framerate = 30; // Default framerate

        private string propertyPath;
        private Type propertyType;
        private string propertyName;


        [MenuItem("CONTEXT/AnimationClip/Open Animation Keyframe Lerper")]
        private static void OpenFromContextMenu(MenuCommand command)
        {
            // Get the selected AnimationClip from the context menu
            AnimationClip clip = (AnimationClip)command.context;

            // Open the editor window
            AnimationKeyframeLerper window = GetWindow<AnimationKeyframeLerper>("Animation Keyframe Lerper");
            window.selectedClip = clip; // Pass the selected clip to the window
        }

        void OnGUI()
        {
            GUILayout.Label("Lerp Animation Keyframes", EditorStyles.boldLabel);

            // Display the selected AnimationClip
            selectedClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", selectedClip, typeof(AnimationClip), false);
            targetPropertyName = EditorGUILayout.TextField("Property Name", targetPropertyName);

            // Input fields for duration and framerate
            duration = EditorGUILayout.FloatField("Duration (seconds):", duration);
            framerate = EditorGUILayout.IntField("Framerate:", framerate);

            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(selectedClip);
            if (bindings.Length == 0)
            {
                EditorGUILayout.LabelField("Bindings:", "None Found.");
            }
            foreach (EditorCurveBinding binding in bindings)
            {
                EditorGUILayout.LabelField("Path: ", binding.path);
                EditorGUILayout.LabelField("Type: ", binding.type.ToString());
                EditorGUILayout.LabelField("Path: ", binding.path);
            }


            if (GUILayout.Button("Apply Lerp"))
            {
                if (selectedClip != null && !string.IsNullOrEmpty(targetPropertyName) && duration > 0 && framerate > 0)
                {
                    ApplyLerpToKeyframes(selectedClip, targetPropertyName, duration, framerate);
                }
                else
                {
                    Debug.LogWarning("Please specify valid inputs for Animation Clip, Property Name, Duration, and Framerate.");
                }
            }
        }

        private void ApplyLerpToKeyframes(AnimationClip clip, string property, float duration, int framerate)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, new EditorCurveBinding
            {
                path = "",
                type = typeof(Transform), // Adjust as needed for your target type
                propertyName = property
            });

            if (curve == null)
            {
                Debug.LogWarning("Property curve not found!");
                return;
            }

            // Calculate keyframe times based on duration and framerate
            int keyframeCount = Mathf.CeilToInt(duration * framerate);
            float timeStep = duration / (keyframeCount - 1);

            // Create new keyframes
            var keyframes = new Keyframe[keyframeCount];
            float startValue = curve.keys[0].value;
            float endValue = curve.keys[curve.keys.Length - 1].value;

            for (int i = 0; i < keyframeCount; i++)
            {
                float time = i * timeStep;
                float t = time / duration; // Normalized time for lerping
                float value = Mathf.Lerp(startValue, endValue, t);

                keyframes[i] = new Keyframe(time, value);
            }

            curve.keys = keyframes;

            AnimationUtility.SetEditorCurve(clip, new EditorCurveBinding
            {
                path = "",
                type = typeof(Transform),
                propertyName = property
            }, curve);

            Debug.Log($"Lerp applied to {clip.name} successfully. Duration: {duration}s, Framerate: {framerate}fps.");
        }

    }
}
