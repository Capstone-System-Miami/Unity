// Author: Alec
using System.Collections.Generic;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    // (Layla) This is a singleton/manager. I wrote a base class for those,
    // that handles the assignement of the static instance & such,
    // so I switched it to inherit from that for consistency.
    public class MapManager : Singleton<MapManager>
    {

        public OverlayTile overlayTilePrefab;
        public GameObject overlayContainer;

        public Dictionary<Vector2Int, OverlayTile> map;

        protected override void Awake()
        {
            base.Awake(); // Handles the assignement of the static instance
        }

        // Start is called before the first frame update
        void Start()
        {
            var tileMap = gameObject.GetComponentInChildren<Tilemap>();
            map = new Dictionary<Vector2Int, OverlayTile> ();

            BoundsInt bounds = tileMap.cellBounds;

            print ($"Bounds xmin {bounds.min.x} | xmax {bounds.max.x}\n" +
                $"ymin {bounds.min.y} | ymax { bounds.max.y}\n" +
                $"zmin { bounds.min.z} | zmax {bounds.max.z}");


            // looping through all of our tiles. --> (Layla) And doing what?
            // It will be easier to debug if we have some notes about what this is
            for (int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        var tileLocation = new Vector3Int(x, y, z);
                        var tileKey = new Vector2Int(x, y);

                        if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                        {
                            var overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                            var cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                            overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z+1);
                            overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.gridLocation = tileLocation;
                            map.Add(tileKey, overlayTile);
                        }
                    }
                }
            }
        }


    }
}
