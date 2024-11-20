// Author: Alec
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, ITargetable
    {
        #region PUBLIC VARS
        // ==================================
        // ??
        public int G;
        public int H;

        // Reference to the character currently on this tile
        public Combatant CurrentCharacter;

        public OverlayTile PreviousTile;
        // ==================================
        #endregion // PUBLIC VARS

        #region PRIVATE
        // ==================================
        private Vector3Int gridLocation;
        private SpriteRenderer _renderer;
        private Color _defaultColor;

        private bool isBlocked;       
        #endregion // PRIVATE VARS ==========

        #region PROPERTIES
        // ==================================

        // Location on the game board in tile units.
        // Cast this to Vector2Int to use it as a key
        // for the map dict
        public Vector3Int GridLocation { get { return gridLocation; } set { gridLocation = value; } }

        /// <summary>
        /// Whether the tile is blocked, e.g. by a character occupier.
        /// </summary>
        public bool Valid
        {
            get
            {
                return (CurrentCharacter == null) && (!isBlocked);
            }
        }

        // ??
        public int F { get { return G + H; } }

        #endregion // PROPERTIES ============

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
        }

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
            HideTile();

            _renderer.color = _defaultColor;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }

        #endregion // ITARGETABLE ===========
    }
}
