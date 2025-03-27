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
    // If assigned, ither copy & replace or create an override
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
            
            var objs = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (var o in objs)
            {
                if (o is Sprite sprite)
                    spritesFound.Add(sprite);
            }
        }

        
        spritesFound.Sort((a, b) => a.name.CompareTo(b.name));

        if (spritesFound.Count == 0)
        {
            Debug.LogError("No sprites found in the specified folder or sheet.");
            return;
        }

        
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

        
        if (baseAnimatorController == null)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Created {createdClips.Count} clips. No base controller was provided.");
            return;
        }

        // If we have a base controller, do the “default” or “override” logic
        if (useAsDefaultController)
        {
            
            string baseControllerPath = AssetDatabase.GetAssetPath(baseAnimatorController);
            string copiedControllerPath = Path.Combine(newFolderPath, newControllerName + ".controller");
            AssetDatabase.CopyAsset(baseControllerPath, copiedControllerPath);
            AssetDatabase.ImportAsset(copiedControllerPath);

            
            AnimatorController copiedController = AssetDatabase.LoadAssetAtPath<AnimatorController>(copiedControllerPath);


            AnimatorControllerLayer layer = copiedController.layers[0];
            
                AnimatorStateMachine sm = layer.stateMachine;
                foreach (var childState in sm.states)
                {
                    var state = childState.state;
                    
                    if (state.motion is AnimationClip)
                    {
                        AnimationClip bestClip = FindBestClipForState(state.name, createdClips);
                        if (bestClip != null)
                        {
                            state.motion = bestClip;
                        }
                    }
                }
            

            Debug.Log(
                $"Copied base controller into '{copiedControllerPath}' and replaced states by name-matching with {createdClips.Count} new clips."
            );
        }
        else
        {
            
            AnimatorOverrideController newOverride = new AnimatorOverrideController(baseAnimatorController);

           
            List<KeyValuePair<AnimationClip, AnimationClip>> overridesList = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            newOverride.GetOverrides(overridesList);

            // For each base clip, find the best new clip
            for (int i = 0; i < overridesList.Count; i++)
            {
                AnimationClip baseClip = overridesList[i].Key;
                if (baseClip == null) continue;

                AnimationClip matchedClip = FindBestClipForState(baseClip.name, createdClips);
                if (matchedClip != null)
                {
                    overridesList[i] = new KeyValuePair<AnimationClip, AnimationClip>(baseClip, matchedClip);
                }
            }

            // Apply & save
            newOverride.ApplyOverrides(overridesList);

            string overridePath = Path.Combine(newFolderPath, newControllerName + ".overrideController");
            AssetDatabase.CreateAsset(newOverride, overridePath);

            Debug.Log(
                $"Created Animator Override Controller '{overridePath}' via name-matching. " +
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
        settings.loopTime = false;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        return clip;
    }

    /// <summary>
    /// Finds the best matching clip out of 'candidates' for a given 'stateName'.
    /// - If there's an EXACT canonical match, score = 2 (highest).
    /// - Else if there's a partial match (clipCan contained in stateCan, or vice versa), score = 1.
    /// - Otherwise, score = 0 (no match).
    /// Returns the clip with the highest score. Ties go to the first found.
    /// </summary>
    private AnimationClip FindBestClipForState(string stateName, List<AnimationClip> candidates)
    {
        // Convert the state's name
        string stateCan = CanonicalName(stateName);

        int bestScore = 0;
        AnimationClip bestClip = null;

        foreach (var clip in candidates)
        {
            string clipCan = CanonicalName(clip.name);
            int score = 0;

            if (clipCan == stateCan)
            {
                // EXACT match => highest priority
                score = 2;
            }
            else
            {
                // PARTIAL match => if either name is contained in the other
                if (stateCan.Contains(clipCan) || clipCan.Contains(stateCan))
                {
                    score = 1;
                }
            }

            if (score > bestScore)
            {
                bestScore = score;
                bestClip = clip;

                // If we found an exact match (score=2), we can break immediately
                if (bestScore == 2)
                {
                    break;
                }
            }
        }

        return bestClip;
    }

    /// <summary>
    /// Converts a string (like a clip or state name) to a "canonical" form
    /// by lowercasing and removing underscores, spaces, and hyphens.
    /// </summary>
    private string CanonicalName(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "";
       
        string result = raw.ToLower();
        
        result = result.Replace("_", "");
        result = result.Replace(" ", "");
        result = result.Replace("-", "");
        return result;
    }
}



