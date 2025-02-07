//Alec's script, layla minor edits

using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class ArrowDrawer : Singleton<ArrowDrawer>
    {
        public GameObject arrowPrefab;
        public List<GameObject> arrows;
        public List<OverlayTile> previousPath;

        protected override void Awake()
        {
            base.Awake();

            arrows = new List<GameObject>();
            previousPath = new List<OverlayTile>();
        }

        public void DrawPath(List<OverlayTile> path)
        {
            if(path == null) return;

            if(previousPath == path) return;

            RemoveArrows();

            previousPath = new List<OverlayTile>(path);

            for(int i = 1; i < path.Count; i++)
            {
                int left = i - 1;
                int right = i + 1;
                int last = path.Count - 1;

                OverlayTile currentTile = path[i];
                OverlayTile prev = ( left >= 0 )        ? path[left]  : null;
                OverlayTile next = ( right <= last )    ? path[right] : null;

                Vector3 arrowsPosition = MapManager.MGR.IsoToScreen(currentTile.GridLocation);

                GameObject arrowGo = Instantiate(arrowPrefab, arrowsPosition, Quaternion.identity);

                Arrow arrow = arrowGo.GetComponent<Arrow>();
                arrow.SetTileData(currentTile, prev, next);

                arrows.Add(arrowGo);
                //todo arrows after path                
            }
            
        }

        public void RemoveArrows()
        {
            foreach (GameObject arrow in arrows)
            {
                Destroy(arrow);
            }
            arrows.Clear();
        }
    }
}
