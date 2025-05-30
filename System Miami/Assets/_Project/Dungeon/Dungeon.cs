/// Layla
using UnityEngine;
using SystemMiami.Utilities;
using UnityEngine.Tilemaps;

namespace SystemMiami.Dungeons
{
    /// <summary>
    /// A Mono Component to be attached to
    /// the root GameObject of every Dungeon Prefab
    /// (i.e. the one at the top of the hierarchy
    /// that contains a `Grid` Component)
    /// </summary>
    /// 
    /// TODO:
    /// I'm sure there will be more info
    /// that needs to be in here.
    public class Dungeon : MonoBehaviour
    {
        [Tooltip(
            "The general style / vibe of the Dungeon. " +
            "This will be used by DungeonEntrance when " +
            "it checks what prefabs it is " +
            "allowed to / prohibited from " +
            "loading when the Neighborhood is generated." )]
        [SerializeField] private Style _style;

        [SerializeField] private BlockHeight _boardTilesHeight;

        [SerializeField] private GameObject _gameBoard;

        [SerializeField] private GameObject _obstacles;

        [SerializeField] private Tilemap staticUndamageable;
        [SerializeField] private Tilemap staticDamageable;
        [SerializeField] private Tilemap dynamicUndamageable;
        [SerializeField] private Tilemap dynamicDamageable;

        private GameObject overlayTileContainer;

        [SerializeField,ReadOnly] public DifficultyLevel DifficultyLevel;

        private void Awake()
        {
            overlayTileContainer = Instantiate(new GameObject(), transform);
            overlayTileContainer.name = "Overlay Tile Container";
        }

        /// <summary>
        /// The general style / vibe of the Dungeon.
        /// </summary>
        /// 
        /// This will be used by DungeonEntrance when
        /// it checks what prefabs it is
        /// allowed to / prohibited from loading
        /// when the Neighborhood is generated.
        public Style Style { get { return _style; } }

        public DungeonRewards Rewards;
        public BlockHeight BoardTilesHeight { get { return _boardTilesHeight; } }

        public GameObject GameBoard { get { return _gameBoard; } }

        public Tilemap StaticUndamageable { get { return staticUndamageable; } }
        public Tilemap StaticDamageable { get { return staticDamageable; } }
        public Tilemap DynamicUndamageable { get { return dynamicUndamageable; } }
        public Tilemap DynamicDamageable { get { return dynamicDamageable; } }

        public GameObject OverlayTileContainer { get { return overlayTileContainer; } }
    }
}
