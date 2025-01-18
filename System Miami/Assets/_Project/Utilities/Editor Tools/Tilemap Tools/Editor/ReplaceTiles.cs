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


        [MenuItem("Tools/Replace Tiles")]
        public static void ShowWindow()
        {
            GetWindow<ReplaceTiles>($"ReplaceTiles");
        }

        private void OnGUI()
        {
            GUILayout.Label($"Replace Tiles", EditorStyles.boldLabel);

            GUILayout.Box($"Replace all instances of a tile with another tile in the selected tilemaps");

            /// Target Tilemaps
            /// ==========================
            GUILayout.Label($"Target Tilemaps", EditorStyles.boldLabel);
            EditorButton addButton;
            EditorButton removeMapButton;

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
                removeMapButton = new("Remove Target Tilemap");
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


            /// Tile Replacement
            /// ==========================
            GUILayout.Label("Tile to Replace", EditorStyles.miniBoldLabel);
            toReplace = (Tile)EditorGUILayout.ObjectField("Tile to Replace", toReplace, typeof(Tile), false);

            GUILayout.Label("Replacement Tile", EditorStyles.miniBoldLabel);
            replacement = (Tile)EditorGUILayout.ObjectField("Replacement Tile", replacement, typeof(Tile), false);

            if (GUILayout.Button("Press To Replace"))
            {
                replaceTiles(targetTilemaps, toReplace, replacement);
            }
        }

        private void replaceTiles(List<Tilemap> maps, Tile toReplace, Tile replacement)
        {
            Debug.Log("Button Clicked");
            //foreach (Tilemap map in maps)
            //{
            //    if (map == null)
            //    {
            //        Debug.LogWarning("Tilemap is null");
            //        continue;
            //    }

            //    Vector3Int min = map.cellBounds.min;
            //    Vector3Int max = map.cellBounds.max;

            //    for (int x = min.x; x < max.x; x++)
            //    {
            //        for (int y = min.y; y < max.y; y++)
            //        {
            //            for (int z = min.z; z < max.z; z++)
            //            {
            //                Vector3Int pos = new Vector3Int(x, y, 0);
            //                Tile tile = map.GetTile<Tile>(pos);

            //                if (tile == toReplace)
            //                {
            //                    map.SetTile(pos, replacement);
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}
