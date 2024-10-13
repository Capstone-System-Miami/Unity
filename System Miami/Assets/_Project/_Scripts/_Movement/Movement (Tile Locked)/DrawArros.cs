using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class DrawArrows : MonoBehaviour
    {
        public static DrawArrows Instance;
        public GameObject arrowPrefab;
        public List<GameObject> arrows;
        public List<OverlayTile> previousPath;

        private void Awake(){ Instance = this; arrows = new List<GameObject>(); previousPath = new List<OverlayTile>(); }

        public void DrawPath(List<OverlayTile> path)
        {
            if(path == null) return;

            if(previousPath == path) return;

            foreach(GameObject arrow in arrows){
                Destroy(arrow);
            }
            arrows.Clear();

            previousPath = new List<OverlayTile>(path);

            for(int i = 1; i < path.Count; i++)
            {
                OverlayTile tile = path[i];

                Vector3 arrowsPosition = MapManager.MGR.IsoToScreen(tile.gridLocation);

                var arrowGo = Instantiate(arrowPrefab, arrowsPosition, Quaternion.identity);

                Arrow arrow = arrowGo.GetComponent<Arrow>();
                arrow.SetTileData(tile, path[i - 1], i < path.Count - 1 ? path[i + 1] : null);

                arrows.Add(arrowGo);
            }
        }
    }
}
