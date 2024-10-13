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
        // Just an enum for the height of the block,
        // which is distinct from the zIndex
        public BlockHeight gridTilesHeight;

        public OverlayTile overlayTilePrefab;
        public GameObject overlayContainer;

        //dictionary containing all tiles on map by their x, y coordinates
        public Dictionary<Vector2Int, OverlayTile> map;

        protected override void Awake()
        {
            base.Awake(); // Handles the assignement of the static instance
        }

        void Start()
        {
            Tilemap tileMap = gameObject.GetComponentInChildren<Tilemap>();
            map = new Dictionary<Vector2Int, OverlayTile> ();

            //get the bounds of tile map in grid coordinates
            BoundsInt bounds = tileMap.cellBounds;

            print ($"Bounds xmin {bounds.min.x} | xmax {bounds.max.x}\n" +
                $"ymin {bounds.min.y} | ymax { bounds.max.y}\n" +
                $"zmin { bounds.min.z} | zmax {bounds.max.z}");


            // Looping through all of our tiles.
            // For each tile found in the tilemap, it instantiates an overlay tile
            // at the world _gridPosition (Calculated by static class Coordinates).
            // Stores the overlay tile in a dictionary letting us access each tile by its x and y.
            for (int z = bounds.max.z; z >= bounds.min.z; z--)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    for (int x = bounds.min.x; x < bounds.max.x; x++)
                    {
                        Vector3Int tileLocation = new Vector3Int(x, y, z);
                        Vector2Int tileKey = new Vector2Int(x, y);

                        if (tileMap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                        {
                            OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);

                            // Using the function from SystemMiami.Coordinates rather than the built in worldpos tilemap fn
                            Vector3 cellWorldPosition = Coordinates.IsoToScreen(tileLocation, gridTilesHeight);

                            overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z);
                            overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tileMap.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.gridLocation = tileLocation;
                            map.Add(tileKey, overlayTile);
                        }
                    }
                }
            }
        }

        public Vector3 IsoToScreen(Vector3Int tileLocation){
            return Coordinates.IsoToScreen(tileLocation, gridTilesHeight);
        }
    }
}
