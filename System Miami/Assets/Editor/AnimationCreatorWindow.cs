using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;

public class AnimationCreatorWindow : EditorWindow
{
    [Header("Sprite Source")]
    private Object spriteFolderOrSheet;

    // How many frames per animation clip
    private int framesPerClip = 6;

    private int clipCount = 8;

    // For naming each clip individually
    private List<string> clipNames = new List<string>();

    // The base Animator Controller
    // If assigned, either copy & replace or create an override
    private AnimatorController baseAnimatorController;
    private bool useAsDefaultController = false; // If true => copy & replace

    private string newControllerName = "NewController";
    private float frameRate = 12f;

    private List<AnimationClip> createdClips = new List<AnimationClip>();

    [MenuItem("Window/Animation Creator")]
    private static void ShowWindow()
    {
        var window = GetWindow<AnimationCreatorWindow>("Animation Creator");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Source", EditorStyles.boldLabel);

        // folder with sprite sheet or sprite sheet itself
        spriteFolderOrSheet = EditorGUILayout.ObjectField(
            "Sprite Folder/Sheet:",
            spriteFolderOrSheet,
            typeof(Object),
            false
        );

        // Frames per clip
        framesPerClip = EditorGUILayout.IntField("Frames per Clip:", framesPerClip);

        GUILayout.Label($"Clip Count: {clipCount}");
        UpdateClipNamesList();

        GUILayout.Space(5);
        GUILayout.Label("Clip Names:", EditorStyles.boldLabel);
        for (int i = 0; i < clipNames.Count; i++)
        {
            clipNames[i] = EditorGUILayout.TextField($"Clip {i + 1} Name:", clipNames[i]);
        }

        GUILayout.Space(10);
        GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
        frameRate = EditorGUILayout.FloatField("Frame Rate:", frameRate);

        GUILayout.Space(10);
        GUILayout.Label("Animator Controller Settings", EditorStyles.boldLabel);

        // Base Animator Controller
        baseAnimatorController = (AnimatorController)EditorGUILayout.ObjectField(
            "Base Animator Controller:",
            baseAnimatorController,
            typeof(AnimatorController),
            false
        );

        // Only show the checkbox if a base controller is assigned
        if (baseAnimatorController != null)
        {
            useAsDefaultController = EditorGUILayout.Toggle(
                "Use as Default (copy & replace)?",
                useAsDefaultController
            );
        }

        newControllerName = EditorGUILayout.TextField("New Controller Name:", newControllerName);

        GUILayout.Space(20);

        if (GUILayout.Button("Create Animations"))
        {
            CreateAnimations();
        }
    }

    /// <summary>
    /// Resizes clipNames to match the clipCount.
    /// </summary>
    private void UpdateClipNamesList()
    {
        int neededCount = clipCount < 0 ? 0 : clipCount;

        // Grow or shrink clipNames to match neededCount
        while (clipNames.Count < neededCount)
        {
            clipNames.Add($"Clip_{clipNames.Count + 1}");
        }
        while (clipNames.Count > neededCount)
        {
            clipNames.RemoveAt(clipNames.Count - 1);
        }
    }

    private void CreateAnimations()
    {
        createdClips.Clear();

        if (spriteFolderOrSheet == null)
        {
            Debug.LogError("Please assign a folder of sprites or a sliced sprite sheet.");
            return;
        }
        if (framesPerClip <= 0)
        {
            Debug.LogError("Frames per Clip must be > 0.");
            return;
        }

        // Collect all Sprites
        List<Sprite> spritesFound = new List<Sprite>();
        string assetPath = AssetDatabase.GetAssetPath(spriteFolderOrSheet);

        if (Directory.Exists(assetPath))
        {
            // It's a folder
            string[] files = Directory.GetFiles(assetPath, "*.png", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var objs = AssetDatabase.LoadAllAssetsAtPath(file);
                foreach (var o in objs)
                {
                    if (o is Sprite sprite)
                        spritesFound.Add(sprite);
                }
            }
        }
        else
        {
            // It's likely a single sprite sheet
            var objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var o in objs)
            {
                if (o is Sprite sprite)
                    spritesFound.Add(sprite);
            }
        }

        // First: Sort the sprites numerically by their name => ensures correct frame order
        spritesFound.Sort((a, b) =>
        {
            int numA = ExtractNumberFromName(a.name);
            int numB = ExtractNumberFromName(b.name);
            return numA.CompareTo(numB);
        });

        if (spritesFound.Count == 0)
        {
            Debug.LogError("No sprites found in the specified folder or sheet.");
            return;
        }

        // Create a new folder for the controller + clips
        string parentFolder = Path.GetDirectoryName(assetPath);
        string newFolderPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(parentFolder, newControllerName)
        );
        AssetDatabase.CreateFolder(parentFolder, newControllerName);

        int actualTotal = spritesFound.Count;
        int actualClipCount = actualTotal / framesPerClip; // integer division

        int usedClipCount = Mathf.Min(actualClipCount, clipNames.Count);

        int currentSpriteIndex = 0;
        for (int i = 0; i < usedClipCount; i++)
        {
            Sprite[] chunk = new Sprite[framesPerClip];
            spritesFound.CopyTo(currentSpriteIndex, chunk, 0, framesPerClip);
            currentSpriteIndex += framesPerClip;

            string clipName = clipNames[i];
            AnimationClip newClip = CreateAnimationClip(chunk, clipName, frameRate);

            string clipPath = Path.Combine(newFolderPath, clipName + ".anim");
            AssetDatabase.CreateAsset(newClip, clipPath);

            createdClips.Add(newClip);
        }

        // If no base controller, we’re done with just the clips
        if (baseAnimatorController == null)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Created {createdClips.Count} clips. No base controller was provided.");
            return;
        }

        // ===============================
        // ADD THE CLIPS TO AN OVERRIDE CONTROLLER 
        // IN ALPHABETICAL ORDER
        // ===============================

        if (useAsDefaultController)
        {
            // Copy base controller
            string baseControllerPath = AssetDatabase.GetAssetPath(baseAnimatorController);
            string copiedControllerPath = Path.Combine(newFolderPath, newControllerName + ".controller");
            AssetDatabase.CopyAsset(baseControllerPath, copiedControllerPath);
            AssetDatabase.ImportAsset(copiedControllerPath);

            AnimatorController copiedController = AssetDatabase.LoadAssetAtPath<AnimatorController>(copiedControllerPath);
            AnimatorControllerLayer layer = copiedController.layers[0];
            var sm = layer.stateMachine;

            // Sort the newly created clips ALPHABETICALLY
            createdClips.Sort((x, y) => x.name.CompareTo(y.name));

            // Now go through each state in alphabetical order of the newly created clips
            var states = sm.states;

            // Option A: Just iterate over states in the order they appear,
            // and match them up with clips in alphabetical order:
            states = SortStateArrayByName(states);

            for (int i = 0; i < states.Length; i++)
            {
                var state = states[i].state;
                if (i < createdClips.Count)
                {
                    state.motion = createdClips[i];
                }
            }

            // (If you prefer to do partial if states < createdClips, you can tweak logic above)

            Debug.Log(
                $"Copied base controller into '{copiedControllerPath}' " +
                $"and assigned {createdClips.Count} new clips (alphabetically) to states."
            );
        }
        else
        {
            // Create an override controller
            AnimatorOverrideController newOverride = new AnimatorOverrideController(baseAnimatorController);

            // Sort the newly created clips ALPHABETICALLY
            createdClips.Sort((x, y) => x.name.CompareTo(y.name));

            // Retrieve the base override
            List<KeyValuePair<AnimationClip, AnimationClip>> overridesList =
                new List<KeyValuePair<AnimationClip, AnimationClip>>();
            newOverride.GetOverrides(overridesList);

            // If you want, also sort "overridesList" by the base clip’s name so that
            // the override list is shown in alphabetical order in the inspector:
            overridesList.Sort((pair1, pair2) => pair1.Key.name.CompareTo(pair2.Key.name));

            // Assign each override in alphabetical order of created clips
            for (int i = 0; i < overridesList.Count; i++)
            {
                // If we have a new clip for this index, assign it
                if (i < createdClips.Count)
                {
                    AnimationClip baseClip = overridesList[i].Key;
                    overridesList[i] = new KeyValuePair<AnimationClip, AnimationClip>(baseClip, createdClips[i]);
                }
            }

            newOverride.ApplyOverrides(overridesList);

            // Save
            string overridePath = Path.Combine(newFolderPath, newControllerName + ".overrideController");
            AssetDatabase.CreateAsset(newOverride, overridePath);

            Debug.Log(
                $"Created Animator Override Controller '{overridePath}' in alphabetical order. " +
                $"Overrode with {createdClips.Count} new clips."
            );
        }

        // Final refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Finished creating {createdClips.Count} animation clips in: {newFolderPath}");
    }

    /// <summary>
    /// Creates an AnimationClip from the given sprites at the specified FPS, looping by default.
    /// </summary>
    private AnimationClip CreateAnimationClip(Sprite[] sprites, string clipName, float fps)
    {
        AnimationClip clip = new AnimationClip();
        clip.name = clipName;
        clip.frameRate = fps;

        EditorCurveBinding curveBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        // Build keyframes
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];
        float timePerFrame = 1f / fps;
        for (int i = 0; i < sprites.Length; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i * timePerFrame,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = false; // example: non-looping
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        return clip;
    }

    /// <summary>
    /// Helper method that tries to parse a trailing integer from a sprite name.
    /// E.g. "sprite_10" => 10, "sprite_2" => 2. If no number is found, returns 0.
    /// </summary>
    private static int ExtractNumberFromName(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
            return 0;

        // Walk backwards to find where digits end
        for (int i = spriteName.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(spriteName[i]))
            {
                // We've hit the first non-digit. If there are digits after it, parse them.
                if (i < spriteName.Length - 1)
                {
                    string numberString = spriteName.Substring(i + 1);
                    if (int.TryParse(numberString, out int parsed))
                        return parsed;
                }
                return 0;
            }

            // If the entire string is numeric (e.g. "10"), and we never hit a non-digit:
            if (i == 0)
            {
                if (int.TryParse(spriteName, out int parsed))
                    return parsed;
            }
        }

        return 0;
    }

    /// <summary>
    /// Sorts an array of ChildAnimatorState by the state's name, returning a new sorted array.
    /// </summary>
    private static ChildAnimatorState[] SortStateArrayByName(ChildAnimatorState[] states)
    {
        List<ChildAnimatorState> list = new List<ChildAnimatorState>(states);
        list.Sort((s1, s2) => s1.state.name.CompareTo(s2.state.name));
        return list.ToArray();
    }
}
