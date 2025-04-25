// Author: Alec, Lee, Layla
using System.Collections.Generic;
using SystemMiami.Management;
using SystemMiami.Utilities;
using SystemMiami.CombatSystem;
using SystemMiami.Dungeons;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

namespace SystemMiami
{
    // (Layla) This is a singleton/manager. I wrote a base class for those,
    // that handles the assignement of the static instance & such,
    // so I switched it to inherit from that for consistency.
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] private dbug log;

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
        private Tilemap staticUndamageable;
        private Tilemap staticDamageable;
        private Tilemap dynamicUndamageable;
        private Tilemap dynamicDamageable;
        private int obstacleTotal;

        private BoundsInt bounds;
        private GameObject overlayContainer;

        public TileCorners TileCorners { get; private set; }

        public int SizeInTileUnits {
            get
            {
                Assert.IsTrue(
                    Mathf.Sqrt(map.Count) == (int)Mathf.Sqrt(map.Count),
                    $"{name} is not using a square game board." +
                    $"This breaks the game.");

                return (int)Mathf.Sqrt(map.Count);
            }
        }

        public Dungeon Dungeon { get { return dungeon; } }

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
                log.error($"{name}'s {this} didn't find " +
                    $"an environment to use");
                return;
            }

            if (!environment.TryGetComponent(out dungeon))
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a Dungeon on the environment object");
                return;
            }

            if (dungeon.OverlayTileContainer == null)
            {
                log.error($"{name}'s {this} didn't find " +
                    $"an Overlay Container in {environment}'s {dungeon}");
                return;
            }

            if (dungeon.GameBoard == null)
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            else if (!dungeon.GameBoard.TryGetComponent(out gameBoardTilemap))
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a tilemap component on {dungeon}'s {dungeon.GameBoard}");
                return;
            }


            if (dungeon.StaticUndamageable != null)
            {
                this.staticUndamageable = dungeon.StaticUndamageable;
            }
            else
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            if (dungeon.StaticDamageable != null)
            {
                this.staticDamageable = dungeon.StaticUndamageable;
            }
            else
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            if (dungeon.DynamicUndamageable != null)
            {
                this.staticUndamageable = dungeon.StaticUndamageable;
            }
            else
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }
            if (dungeon.DynamicDamageable != null)
            {
                this.staticUndamageable = dungeon.StaticUndamageable;
                this.staticDamageable = dungeon.StaticDamageable;
                this.dynamicUndamageable = dungeon.DynamicUndamageable;
                this.dynamicDamageable = dungeon.DynamicDamageable;
            }
            else
            {
                log.error($"{name}'s {this} didn't find " +
                    $"a GameBoard object in {environment}'s {dungeon}");
                return;
            }


            // Construction
            gridTilesHeight = dungeon.BoardTilesHeight;
            overlayContainer = dungeon.OverlayTileContainer;
            map = new Dictionary<Vector2Int, OverlayTile> ();

            // Get the bounds of tile map in BounsInt
            bounds = gameBoardTilemap.cellBounds;

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

                            if (GAME.MGR.IgnoreObstacles)
                            {
                                staticUndamageable?.gameObject.SetActive(false);
                                staticDamageable?.gameObject.SetActive(false);
                                dynamicUndamageable?.gameObject.SetActive(false);
                                dynamicDamageable?.gameObject.SetActive(false);
                            }
                            else
                            {
                                Obstacle obstacle = null;
                                if (!TryPlaceObstacle(staticUndamageable, overlayTile, out obstacle))
                                {
                                    log.warn($"Couldn't place su obst on {overlayTile}.");
                                }
                                else if (!TryPlaceObstacle(staticDamageable, overlayTile, out obstacle))
                                {
                                    log.warn($"Couldn't place sd obst on {overlayTile}.");
                                }
                                else if (!TryPlaceObstacle(dynamicUndamageable, overlayTile, out obstacle))
                                {
                                    log.warn($"Couldn't place du obst on {overlayTile}.");
                                }
                                else if (!TryPlaceObstacle(dynamicDamageable, overlayTile, out obstacle))
                                {
                                    log.warn($"Couldn't place dd obst on {overlayTile}.");
                                }
                            }
                        }
                    }
                }
            }

            InitCenter();
            InitCorners();
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
                log.error(
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

        private bool TryPlaceObstacle(Tilemap obstacleMap, OverlayTile tile, out Obstacle obstacle)
        {
            obstacle = null;

            // obstacle placement
            Vector3Int posToCheck = new (tile.BoardPos.x, tile.BoardPos.y, 2);

            if (!obstacleMap.HasTile(posToCheck))
            {
                return false;
            }
            else
            {
                TileData tileData = new();
                TileBase tileBase = obstacleMap.GetTile(posToCheck);
                tileBase.GetTileData(posToCheck, obstacleMap, ref tileData);
                obstacleMap.SetTile(posToCheck, null);

                Sprite obstacleSprite = tileData.sprite;

                GameObject newObstacleObj = new($"OBSTACLE {obstacleTotal}");
                newObstacleObj.transform.SetParent(obstacleMap.transform);
                obstacle = newObstacleObj.AddComponent<DynamicObstacle>();
                obstacle.Initialize(obstacleSprite);
                return TryPlaceOnTile(obstacle, tile);
            }
        }

        private void InitCorners()
        {
            List<Vector2Int> orderByX = map.Keys.OrderBy(vec => vec.x).ToList();
            List<Vector2Int> orderByY = map.Keys.OrderBy(vec => vec.y).ToList();

            int lastTileIndex = map.Count - 1;

            Vector2Int bottom = new(
                orderByX[0].x,
                orderByY[0].y
            );

            Vector2Int top = new(
                orderByX[lastTileIndex].x,
                orderByY[lastTileIndex].y
            );

            Vector2Int left = new(
                orderByX[0].x,
                orderByY[lastTileIndex].y
            );

            Vector2Int right = new(
                orderByX[lastTileIndex].x,
                orderByY[0].y
            );

            TileCorners = new(map[bottom], map[top], map[left], map[right]);
        }

        private void InitCenter()
        {
            Assert.IsTrue(
                SizeInTileUnits % 2 != 0,
                $"The game board's size is not odd. This could cause " +
                $"issues with many systems.");

            CenterPos = new(SizeInTileUnits, SizeInTileUnits);
        }
    }
}
