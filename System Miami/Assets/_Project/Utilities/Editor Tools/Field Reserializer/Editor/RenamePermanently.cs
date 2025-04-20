using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemMiami.Utilities;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.CustomEditor
{
    public class RenamePermanently : EditorWindow
    {
        private List<GameObject> targetPrefabs = new();
        private List<MonoBehaviour> targetComponents = new();

        private Dictionary<string, string> fieldPairs = new();
        private List<string> namesOfFieldsToReplace;
        private List<string> replacementFieldNames;

        private MonoScript script;

        private EditorButton resetButton;
        private EditorButton replaceButton;

        private static FindPrefabsWithScriptWindow findPrefabsWindow;


        [MenuItem("Tools/Rename Fields Permanently")]
        public static void ShowThisAndPrefabFinderWindow()
        {
            GetWindow<RenamePermanently>($"RenamePermanently");
            findPrefabsWindow = GetWindow<FindPrefabsWithScriptWindow>($"Find Prefabs with Script");
        }

        void OnEnable()
        {

        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            /// Header
            /// ==========================
            GUILayout.BeginVertical($"Rename Permanently", "window");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            //GUILayout.Label($"Rename Permanently", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            /// Description
            /// ==========================
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = Color.white;
            boxStyle.hover.textColor = Color.white;
            GUILayout.Box(
                $"Rename fields on scripts while maintaining the " +
                $"references held by any prefabs that have the component.",
                boxStyle);
            GUILayout.EndVertical();
            GUILayout.Space(20);


            /// Target Script to replace fields in
            GUILayout.BeginVertical($"Target Script", "window");
            GUILayout.Space(10);
            script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
            if (script == null)
            {
                GUILayout.Box(
                    "Drag a script that implements the IFieldReserializer " +
                    "interface into the box above.");
                GUILayout.Space(10);
                GUILayout.EndVertical();
                return;
            }
            else if (!typeof(IFieldReserializer).IsAssignableFrom(script.GetClass()))
            {
                EditorGUILayout.HelpBox(
                    "Something went wrong. Ensure the IFieldReserializer " +
                    "interface is implemented on the target script.",
                    MessageType.Error);
                GUILayout.Space(10);
                GUILayout.EndVertical();
                return;
            }
            else
            {
                GUILayout.Space(10);
                GUILayout.EndVertical();
                /// Box to see what fields will be to found & replaced
                GUILayout.BeginVertical("Field Find & Replace by Name", "window");
                GUILayout.Space(10);

                GameObject tempObj = new();
                object componentOfTypeAddedToTempObjAs_obj = tempObj.AddComponent(script.GetClass());
                if (componentOfTypeAddedToTempObjAs_obj is IFieldReserializer obj_castToRsInterface)
                {
                    // Header Cols
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name of Field to Replace");
                    GUILayout.Label("Replacement Field Name");
                    GUILayout.EndHorizontal();

                    Dictionary<string, string> fieldPairs =
                        obj_castToRsInterface.OldFieldName_NewFieldName();

                    // Field Name Pairs
                    for (int i = 0; i < fieldPairs.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        string key = fieldPairs.Keys.ElementAt(i);
                        this.fieldPairs[key] = fieldPairs[key];
                        EditorGUILayout.TextField(key);
                        EditorGUILayout.TextField(this.fieldPairs[key]);
                        EditorGUI.EndDisabledGroup();
                        GUILayout.EndHorizontal();
                    }
                }
                //else if ((script.GetClass() as IFieldReserializer).OldFieldName_NewFieldName == null)
                //{
                //    EditorGUILayout.HelpBox(
                //        $"IFieldReserializer is not returning a dictionary",
                //        MessageType.Error);
                //    GUILayout.EndVertical();
                //    return;
                //}
                //else if ((script.GetClass() as IFieldReserializer).OldFieldName_NewFieldName.Count == 0)
                //{
                //    EditorGUILayout.HelpBox(
                //        $"No fields to replace found in the script.",
                //        MessageType.Warning);
                //    GUILayout.EndVertical();
                //    return;
                //}
                else
                {
                    EditorGUILayout.HelpBox(
                        $"{script.GetClass()}",
                        MessageType.Error);
                    GUILayout.EndVertical();
                    return;
                }
                DestroyImmediate(tempObj);
            }
            GUILayout.Space(10);
            GUILayout.EndVertical();

            /// Target Prefabs
            /// ==========================
            GUILayout.BeginVertical($"TargetPrefabs", "window");
            GUILayout.Space(10);

            if (findPrefabsWindow == null)
            {
                EditorGUILayout.HelpBox(
                    "Search Window var is null. Will try to find...",
                    MessageType.Error);

                findPrefabsWindow = GetWindow<FindPrefabsWithScriptWindow>($"Find Prefabs with Script");
            }
            else
            {
                findPrefabsWindow.FindPrefabsWithScript(script, true);

                List<GameObject> searchResults = findPrefabsWindow.GetCurrentList;
                if (searchResults == null || searchResults.Count == 0)
                {
                    GUILayout.Box("No Results reported from Search Window");
                }
                targetPrefabs.Clear();
                targetComponents.Clear();

                targetPrefabs.AddRange(searchResults);
                targetPrefabs.ForEach(prefab
                    => targetComponents.Add(prefab.GetComponent(script.GetClass()) as MonoBehaviour));
            }

            for (int i = 0; i < targetPrefabs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(
                    targetPrefabs[i],
                    script.GetClass(),
                    true);

                targetComponents[i] = targetPrefabs[i].GetComponent(script.GetClass()) as MonoBehaviour;
                EditorGUILayout.ObjectField(
                    targetComponents[i],
                    script.GetClass(),
                    true);

                GUILayout.EndHorizontal();
            }

            if (targetPrefabs.Count == 0 || targetPrefabs.Count != targetComponents.Count)
            {
                EditorGUILayout.HelpBox(
                    "Something doesn't look right here...", MessageType.Warning);
                return;
            }

            //GUILayout.BeginHorizontal();
            //addPrefabButton = new("Add Target Prefab");
            //addPrefabButton.IsEnabled = !useFoundPrefabs;
            //if (addPrefabButton.Pressed())
            //{
            //    targetPrefabs.Add(null);
            //    targetComponents.Add(null);
            //}

            //if (targetPrefabs.Count == 0)
            //{
            //    removePrefabButton = new("Remove Prefab/Component Pair");
            //    removePrefabButton.IsEnabled = false;
            //    if (removePrefabButton.Pressed())
            //    {
            //        Debug.LogWarning("Nothing to Remove...");
            //    }
            //}
            //else
            //{
            //    removePrefabButton = new($"Remove Prefab/Component Pair {targetPrefabs.Count - 1}");
            //    removePrefabButton.IsEnabled = (!useFoundPrefabs && targetPrefabs.Count > 0);
            //    if (removePrefabButton.Pressed())
            //    {
            //        //Debug.Log($"Removing {targetPrefabs[targetPrefabs.Count - 1]} from targets.");

            //        targetPrefabs.RemoveAt(targetPrefabs.Count - 1);
            //        targetComponents.RemoveAt(targetComponents.Count - 1);
            //    }
            //}
            //GUILayout.EndHorizontal();
            //GUILayout.FlexibleSpace();
            //GUILayout.Space(10);
            ////usePrefabsFromSearchWindow = new("Use Prefabs from Search Window");
            ////usePrefabsFromSearchWindow.IsEnabled = true;
            ////if (usePrefabsFromSearchWindow.Pressed())
            ////{
            ////    useFoundPrefabs = true;
            ////}
            //useFoundPrefabs = GUILayout.Toggle(useFoundPrefabs, "Use Prefabs from Search Window");

            GUILayout.Space(20);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            /// Clear Button
            /// ==========================
            GUILayout.Space(20);
            resetButton = new("Reset", GUILayout.Height(30));
            resetButton.IsEnabled = true;
            if (resetButton.Pressed())
            {
                clear();
                return;
            }

            GUILayout.FlexibleSpace();
            /// vvv  BOTTOM OF WINDOW  vvv

            /// Replace Button
            /// ==========================
            replaceButton = new("Press To Rename", GUILayout.Height(50));
            replaceButton.IsEnabled = true;
            if (replaceButton.Pressed())
            {
                bool success = true;
                int componentsWithReplacedFields = 0;
                int fieldsReplaced = 0;


                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

                Dictionary<FieldInfo, FieldInfo> infos = new();

                // for each field name pair
                for (int i = 0; i < fieldPairs.Count; i++)
                {
                    string key = fieldPairs.Keys.ElementAt(i);
                    string value = fieldPairs.Values.ElementAt(i);

                    // find the field
                    FieldInfo oldInfo = script.GetClass().GetField(key, flags);
                    FieldInfo newInfo = script.GetClass().GetField(value, flags);
                    if (oldInfo == null || newInfo == null
                        || oldInfo.FieldType != newInfo.FieldType)
                    {
                        success = false;
                        break;
                    }

                    infos.Add(oldInfo, newInfo);
                }

                if (success)
                {
                    for (int i = 0; i < targetComponents.Count; i++)
                    {
                        for (int j = 0; j < infos.Count; j++)
                        {
                            FieldInfo oldInfo = infos.Keys.ElementAt(j);
                            FieldInfo newInfo = infos.Values.ElementAt(j);
                            newInfo.SetValue(targetComponents[i], oldInfo.GetValue(targetComponents[i]));
                            oldInfo.SetValue(targetComponents[i], null);
                            EditorUtility.SetDirty(targetComponents[i]);
                            EditorUtility.SetDirty(targetPrefabs[i]);

                            fieldsReplaced++;
                        }

                        componentsWithReplacedFields++;
                    }

                    Debug.Log($"Successfully replaced {fieldsReplaced} fields " +
                        $"in {componentsWithReplacedFields} components.");
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        $"Bad. something bad. bad.",
                        MessageType.Error);
                }
            }
        }

        private void clear()
        {
            targetPrefabs.Clear();
            targetComponents.Clear();
            namesOfFieldsToReplace.Clear();
            replacementFieldNames.Clear();
            script = null;
        }
    }
}
