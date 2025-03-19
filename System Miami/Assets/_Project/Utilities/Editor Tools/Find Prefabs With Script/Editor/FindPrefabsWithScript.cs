using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// Author: Modified by Layla Hoey from original code by SonmezCankurt
namespace SystemMiami.CustomEditor
{
    /// <summary>
    /// Heavily borrowed from Github user SonmezCankurt's
    /// FindPrefabsWithScript tool. That repository contains the code this
    /// is based on, and can be found at
    /// <see href="https://github.com/SonmezCankurt/FindPrefabsWithScript/tree/master"/>
    /// Modifications by Layla
    /// </summary>
    public class FindPrefabsWithScriptWindow : EditorWindow
    {
        private MonoScript script;
        private bool searchInChildren = true;
        private bool allowSceneObjects = false;
        private List<GameObject> prefabsWithScript = new List<GameObject>();
        private Vector2 scrollPosition;

        public List<GameObject> GetCurrentList => new(prefabsWithScript);

        [MenuItem("Tools/Find Prefabs with Script")]
        public static void ShowWindow()
        {
            GetWindow<FindPrefabsWithScriptWindow>("Find Prefabs with Script");
        }

        private void OnGUI()
        {
            GUILayout.Label("Find Prefabs with Script", EditorStyles.boldLabel);

            script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
            if (script != null)
            {
                if (script.GetClass() == null)
                {
                    EditorGUILayout.HelpBox("Please select a valid script.", MessageType.Error);
                    prefabsWithScript.Clear();
                    return;
                }
                else if (!script.GetClass().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    EditorGUILayout.HelpBox("Please select a script that is derived from MonoBehaviour.", MessageType.Error);
                    prefabsWithScript.Clear();
                    return;
                }
            }

            searchInChildren = EditorGUILayout.Toggle("Search In Children", searchInChildren);
            allowSceneObjects = EditorGUILayout.Toggle("AllowSceneObjects", allowSceneObjects);

            if (GUILayout.Button("Find"))
            {
                FindPrefabsWithScript();
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (GameObject prefab in prefabsWithScript)
            {
                EditorGUILayout.ObjectField(prefab, typeof(GameObject), true);
            }
            EditorGUILayout.EndScrollView();
        }

        public void FindPrefabsWithScript(MonoScript script, bool searchInChildren)
        {
            this.script = script;
            this.searchInChildren = searchInChildren;
            FindPrefabsWithScript();
        }

        private void FindPrefabsWithScript()
        {
            prefabsWithScript.Clear();

            if (script == null)
                return;

            System.Type scriptType = script.GetClass();
            if (scriptType == null)
                return;

            //string filter = allowSceneObjects ? "t:Prefab" : "t:GameObject, t:Scene, a: assets";
            string filter = "t:Prefab";
            string[] prefabPaths = AssetDatabase.FindAssets(filter);
            foreach (string path in prefabPaths)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(path);
                Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
                if (obj is GameObject)
                {
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (PrefabUtility.IsPartOfPrefabAsset(prefab))
                    {
                        if (searchInChildren)
                        {
                            Component[] components = prefab.GetComponentsInChildren(scriptType, true);
                            if (components.Length > 0)
                            {
                                prefabsWithScript.Add(prefab);
                            }
                        }
                        else
                        {
                            Component component = prefab.GetComponent(scriptType);
                            if (component != null)
                            {
                                prefabsWithScript.Add(prefab);
                            }
                        }
                    }
                }
                //else if (obj is SceneAsset scene)
                //{
                //    Scene sc = SceneManager.GetSceneByPath(assetPath);
                //    GameObject[] rootGameObjects = sc.GetRootGameObjects();

                //    for (int i = 0; i < rootGameObjects.Length; i++)
                //    {
                //        GameObject rootGameObject = rootGameObjects[i];
                //        if (searchInChildren)
                //        {
                //            Component[] components = rootGameObject.GetComponentsInChildren(scriptType, true);
                //            if (components.Length > 0)
                //            {
                //                prefabsWithScript.Add(rootGameObject);
                //            }
                //        }
                //        else
                //        {
                //            Component component = rootGameObject.GetComponent(scriptType);
                //            if (component != null)
                //            {
                //                prefabsWithScript.Add(rootGameObject);
                //            }
                //        }
                //    }
                //}
            }
        }
    }
}