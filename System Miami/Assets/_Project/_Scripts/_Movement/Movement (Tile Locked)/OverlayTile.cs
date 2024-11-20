// Author: Alec
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, ITargetable
    {
        #region PUBLIC VARS
        // ==================================

        // ??
        public int G;
        public int H;

        public OverlayTile PreviousTile;

        #endregion // PUBLIC VARS ===========


        #region SERIALIZED VARS
        // ==================================
        [SerializeField] private Color occupiedColor = Color.white;
        [SerializeField] private Color activeCombatantColor = Color.white;

        #endregion // SERIALIZED VARS =======


        #region PRIVATE VARS
        // ==================================

        private SpriteRenderer _renderer;
        private Color _defaultColor;

        private Vector3Int gridLocation;

        private bool isBlocked;

        private Combatant currentCombatant;
        private Color currentColor;
        
        #endregion // PRIVATE VARS ==========


        #region PROPERTIES
        // ==================================

        // Location on the game board in tile units.
        // Cast this to Vector2Int to use it as a key
        // for the map dict
        public Vector3Int GridLocation { get { return gridLocation; } set { gridLocation = value; } }

        // Reference to the combatant currently on this tile
        public Combatant CurrentCombatant { get { return currentCombatant; } }

        /// <summary>
        /// Whether the tile is blocked, e.g. by a combatant occupier.
        /// </summary>
        public bool Valid
        {
            get
            {
                return (currentCombatant == null) && (!isBlocked);
            }
        }

        // ??
        public int F { get { return G + H; } }

        #endregion // PROPERTIES ============


        #region UNITY
        // ==================================

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
        }

        #endregion // UNITY =================


        #region VISIBILITY
        // ==================================

        public void ShowTile()
        {
            //Debug.Log($"{name} at {gridLocation} is trying to show self");
            // Max vals means white.
            gameObject.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, 1);
        }

        public void HideTile()
        {
            // Alpha = 0 means transparent.
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
        
        #endregion // VISIBILITY ============


        #region ITARGETABLE
        // ==================================

        public void Target()
        {
            ShowTile();
        }

        public void Highlight(Color color)
        {
            ShowTile();

            _renderer.color = color;
        }

        public void UnHighlight()
        {
            if (currentCombatant != null)
            {
                _renderer.color = occupiedColor;
            }
            else
            {
                _renderer.color = _defaultColor;
                HideTile();
            }
        }

        public GameObject GameObject()
        {
            return gameObject;
        }

        #endregion // ITARGETABLE ===========


        /// <summary>
        /// Positions a combatant on this tile.
        /// If the combatant already has a CurrentTile,
        /// this function calls RemoveCombatant() on
        /// combatant's CurrentTile before setting it to
        /// this tile.
        /// </summary>
        public void PlaceCombatant(Combatant combatant)
        {
            combatant.transform.position = new Vector3(transform.position.x, transform.position.y + 0.0001f, transform.position.z);

            //combatant.GetComponent<SpriteRenderer>().sortingOrder = tile.GetComponent<SpriteRenderer>().sortingOrder;

            // Let the old tile know that we're gone
            if (combatant.CurrentTile != null)
            {
                combatant.CurrentTile.RemoveCombatant();
            }

            // Show tile
            // set occupied color

            // Update new tile's current combatant
            currentCombatant = combatant;
            combatant.CurrentTile = this;
        }

        public void RemoveCombatant()
        {
            // hide tile,
            // set default color

            currentCombatant.CurrentTile = null;
            currentCombatant = null;
        }
    }
}
