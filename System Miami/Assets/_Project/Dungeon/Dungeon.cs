/// Layla
using UnityEngine;
using SystemMiami.Utilities;

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

        [SerializeField] private GameObject _overlayTileContainer;

        /// <summary>
        /// The general style / vibe of the Dungeon.
        /// </summary>
        /// 
        /// This will be used by DungeonEntrance when
        /// it checks what prefabs it is
        /// allowed to / prohibited from loading
        /// when the Neighborhood is generated.
        public Style Style { get { return _style; } }

        public BlockHeight BoardTilesHeight { get { return _boardTilesHeight; } }

        public GameObject GameBoard { get { return _gameBoard; } }

        public GameObject Obstacles { get { return _obstacles; } }

        public GameObject OverlayTileContainer { get { return _overlayTileContainer; } }
    }
}
