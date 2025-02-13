// Author: Alec, Lee, Layla
using System.Collections.Generic;
using SystemMiami.Management;
using SystemMiami.Utilities;
using SystemMiami.CombatSystem;
using SystemMiami.Dungeons;
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

        //dictionary containing all tiles on map by their x, y coordinates
        public Dictionary<Vector2Int, OverlayTile> map;

        public Vector2Int CenterPos { get; private set; }

        [SerializeField] private GameObject environment;

        private Dungeon dungeon;
        private Tilemap gameBoardTilemap;
        private Tilemap obstaclesTilemap; // This will change, possibly to Obstacle[]
        private BoundsInt bounds;
        private GameObject overlayContainer;

        public BoundsInt Bounds { get { return bounds; } }

        protected override void Awake()
        {
            base.Awake(); // Handles the assignement of the static instance
        }

        void Start()
        {
            // Validation
            if (GAME.MGR.TryGetDungeonPrefab(out GameObject prefab))
            {
                if (environment != null)
                {
                    Destroy(environment);
                }

                environment = Instantiate(prefab);
            }

            if (environment == null)
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"an environment to use");
                return;
            }

            if (!environment.TryGetComponent(out dungeon))
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"a Dungeon on the environment object");
                return;
            }

            if (dungeon.OverlayTileContainer == null)
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"an Overlay Container in {environment}'s {dungeon}");
                return;
            }

            if (dungeon.GameBoard == null)
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            else if (!dungeon.GameBoard.TryGetComponent(out gameBoardTilemap))
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"a tilemap component on {dungeon}'s {dungeon.GameBoard}");
                return;
            }

            if (dungeon.Obstacles == null)
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            else if (!dungeon.Obstacles.TryGetComponent(out obstaclesTilemap))
            {
                Debug.LogError($"{name}'s {this} didn't find " +
                    $"a tilemap component on {dungeon}'s {dungeon.GameBoard}");
                return;
            }



            // Construction
            gridTilesHeight = dungeon.BoardTilesHeight;
            overlayContainer = dungeon.OverlayTileContainer;
            map = new Dictionary<Vector2Int, OverlayTile> ();

            // Get the bounds of tile map in BounsInt
            bounds = gameBoardTilemap.cellBounds;

            Debug.Log(
                $"Bounds\n" +
                $"| xmin {bounds.min.x}\n" +
                $"| xmax {bounds.max.x}\n" +
                $"| ymin {bounds.min.y}\n" +
                $"| ymax { bounds.max.y}\n" +
                $"|zmin { bounds.min.z}\n" +
                $"| zmax {bounds.max.z}"
                );

            CenterPos = GetCenter(bounds);

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

                        if (gameBoardTilemap.HasTile(tileLocation) && !map.ContainsKey(tileKey))
                        {
                            OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                            overlayTile.name = $"OT { tileKey }";

                            // Using the function from SystemMiami.Coordinates rather than the built in worldpos tilemap fn
                            Vector3 cellWorldPosition = Coordinates.IsoToScreen(tileLocation, gridTilesHeight);

                            overlayTile.transform.position = new Vector3(cellWorldPosition.x, cellWorldPosition.y, cellWorldPosition.z);
                            //overlayTile.GetComponent<SpriteRenderer>().sortingOrder = gameBoardTilemap.GetComponent<TilemapRenderer>().sortingOrder;
                            overlayTile.GridLocation = tileLocation;
                            map.Add(tileKey, overlayTile);
                        }
                    }
                }
            }
        }

        public bool TryGetTile(Vector2Int coordinates, out OverlayTile tile)
        {
            if (!map.ContainsKey(coordinates))
            {
                tile = null;
                return false;
            }

            tile = map[coordinates];
            return true;
        }

        /// <summary>
        /// (Lee)
        /// Finds a random unblocked tile on the map.
        /// </summary>
        /// <returns>An unblocked OverlayTile or null if none are available.</returns>
        public OverlayTile GetRandomValidTile()
        {
            // Get all unblocked tiles
            List<OverlayTile> unblockedTiles = new List<OverlayTile>();

            foreach (OverlayTile tile in map.Values)
            {
                if (tile.ValidForPlacement)
                {
                    unblockedTiles.Add(tile);
                }
            }

            if (unblockedTiles.Count > 0)
            {
                // Select a random tile
                int index = UnityEngine.Random.Range(0, unblockedTiles.Count);
                return unblockedTiles[index];
            }

            return null;
        }

        public bool TryPlaceOnTile(ITileOccupant occupant, OverlayTile tile)
        {
            if (!tile.ValidForPlacement)
            {
                Debug.LogError(
                    $"Trying to place {occupant} " +
                    $"on {tile.gameObject}. " +
                    $"This placement is invalid.");
                return false;
            }

            occupant.PositionTile?.ClearOccupant();

            tile.SetOccupant(occupant);
            return true;
        }

        public Vector3 IsoToScreen(Vector3Int tileLocation)
        {
            return Coordinates.IsoToScreen(tileLocation, gridTilesHeight);
        }

        private Vector2Int GetCenter(BoundsInt bounds)
        {
            return new(
                Mathf.RoundToInt(bounds.center.x),
                Mathf.RoundToInt(bounds.center.y)
                );
        }
    }
}
