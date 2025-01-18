using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace SystemMiami.CustomEditor
{
    public class ReplaceTiles : EditorWindow
    {
        private List<Tilemap> targetTilemaps = new();
        private Tile toReplace;
        private Tile replacement;

        private EditorButton addButton;
        private EditorButton removeMapButton;
        private EditorButton swapButton;
        private EditorButton resetButton;


        [MenuItem("Tools/Replace Tiles")]
        public static void ShowWindow()
        {
            GetWindow<ReplaceTiles>($"ReplaceTiles");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            /// Header
            /// ==========================
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label($"Replace Tiles", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            /// Description
            /// ==========================
            GUILayout.Box($"Replace all instances of a tile with another tile in the selected tilemaps");
            GUILayout.Space(20);

            /// Target Tilemaps
            /// ==========================
            GUILayout.Label($"Target Tilemaps", EditorStyles.boldLabel);
            GUILayout.Space(5);

            for (int i = 0; i < targetTilemaps.Count; i++)
            {
                targetTilemaps[i] = (Tilemap)EditorGUILayout.ObjectField($"Target {i}", targetTilemaps[i], typeof(Tilemap), true);
            }

            addButton = new("Add Target Tilemap");
            addButton.IsEnabled = true;
            if (addButton.Pressed())
            {
                targetTilemaps.Add(null);
            }

            if (targetTilemaps.Count < 1)
            {
                removeMapButton = new("Remove Target");
                removeMapButton.IsEnabled = false;
                if (removeMapButton.Pressed())
                {
                    Debug.LogWarning("Nothing to Remove...");
                }
            }
            else
            {
                removeMapButton = new($"Remove Target {targetTilemaps.Count - 1}");
                removeMapButton.IsEnabled = true;
                if (removeMapButton.Pressed())
                {
                    Debug.Log($"Removing {targetTilemaps[targetTilemaps.Count - 1]} from targets.");
                    targetTilemaps.RemoveAt(targetTilemaps.Count - 1);
                }
            }
            GUILayout.Space(20);

            /// Tile Replacement
            /// ==========================
            toReplace = (Tile)EditorGUILayout.ObjectField("Tile to Replace", toReplace, typeof(Tile), false);
            GUILayout.Space(5);

            replacement = (Tile)EditorGUILayout.ObjectField("Replacement Tile", replacement, typeof(Tile), false);
            GUILayout.Space(5);

            /// Swap Button
            /// ==========================
            swapButton = new("Swap");
            swapButton.IsEnabled = (toReplace != null && replacement != null);
            if (swapButton.Pressed())
            {
                swap();
                return;
            }

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
            if (GUILayout.Button("Press To Replace", GUILayout.Height(50)))
            {
                replaceTiles(targetTilemaps, toReplace, replacement);
            }
        }

        private void swap()
        {
            Tile buffer = toReplace;
            toReplace = replacement;
            replacement = buffer;
        }

        private void clear()
        {
            targetTilemaps.Clear();
            toReplace = null;
            replacement = null;

            addButton = null;
            removeMapButton = null;
            swapButton = null;
        }

        private void replaceTiles(List<Tilemap> maps, Tile toReplace, Tile replacement)
        {
            Debug.Log("Button Clicked");
            foreach (Tilemap map in maps)
            {
                if (map == null)
                {
                    Debug.LogWarning("Tilemap is null");
                    continue;
                }

                Vector3Int min = map.cellBounds.min;
                Vector3Int max = map.cellBounds.max;

                for (int x = min.x; x < max.x; x++)
                {
                    for (int y = min.y; y < max.y; y++)
                    {
                        for (int z = min.z; z < max.z; z++)
                        {
                            Vector3Int pos = new Vector3Int(x, y, 0);
                            Tile tile = map.GetTile<Tile>(pos);

                            if (tile == toReplace)
                            {
                                map.SetTile(pos, replacement);
                            }
                        }
                    }
                }
            }
        }
    }
}
