// Author: Alec
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, IHighlightable
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
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _invalidColor = Color.white;
        [SerializeField] private Color _occupiedColor = Color.white;
        [SerializeField] private Color _activeCombatantColor = Color.white;

        #endregion // SERIALIZED VARS =======


        #region PRIVATE VARS
        // ==================================

        private SpriteRenderer _renderer;

        private Vector3Int _gridLocation;

        private bool _hasObstacle;

        private Combatant _currentCombatant;

        private bool _isHovering;

        private bool _isHighlighted;
        private bool _customHighlight;

        private StructSwitcher<Color> _targetColor;
        
        #endregion // PRIVATE VARS ==========


        #region PROPERTIES
        // ==================================

        /// <summary>
        /// Location on the game board in tile units.
        /// Cast this to Vector2Int to use it as a key
        /// for the map dict.
        /// </summary>
        public Vector3Int GridLocation { get { return _gridLocation; } set { _gridLocation = value; } }

        /// <summary>
        /// Reference to the combatant currently occupying this tile
        /// </summary>
        public Combatant CurrentCombatant { get { return _currentCombatant; } }

        /// <summary>
        /// Whether or not the tile has an obstacle on it.
        /// </summary>
        public bool HasObstacle
        {
            get
            {
                return _hasObstacle;
            }
        }

        /// <summary>
        /// Whether or not the tile is occupied by a combatant.
        /// </summary>
        public bool Occupied
        {
            get
            {
                return _currentCombatant != null;
            }
        }

        /// <summary>
        /// Whether something can be positioned onto the tile.
        /// </summary>
        public bool ValidForPlacement
        {
            get
            {
                return !HasObstacle && !Occupied;
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
            _targetColor = new StructSwitcher<Color>(_defaultColor);
        }

        private void Start()
        {
            UnHighlight();
        }

        private void Update()
        {
            Color highlighted = getHighlightedColor();
            Color unhighlighted = getUnhighlightedColor();

            if (!_customHighlight)
            {
                _targetColor.Set(_isHighlighted ? highlighted : unhighlighted);
            }

            if (_renderer.color != _targetColor.Get())
            {
                _renderer.color = _targetColor.Get();
            }
        }

        #endregion // UNITY =================


        #region VISIBILITY
        // ==================================

        private Color getHighlightedColor()
        {
            if (ValidForPlacement)
            {
                return _defaultColor;
            }
            else
            {
                return _invalidColor;
            }
        }

        private Color getUnhighlightedColor()
        {
            if (Occupied)
            {
                if (CurrentCombatant.Controller.IsMyTurn)
                {
                    return _activeCombatantColor;
                }
                else
                {
                    return _occupiedColor;
                }
            }

            // Alpha set to 0 means transparent.
            return new Color(1, 1, 1, 0);
        }

        #endregion // VISIBILITY ============


        #region MOUSEOVER
        // ==================================

        public void BeginHover(PlayerController controller)
        {
            if (Occupied)
            {
                Highlight();
                CurrentCombatant.Highlight();
            }

            if (!controller.IsMyTurn) { return; }

            if (controller.HasActed) { return; }

            Highlight();
        }

        public void EndHover(PlayerController controller)
        {
            if (_customHighlight)
            {
                _targetColor.Revert();
            }
            else
            {
                UnHighlight();
            }

            if (Occupied)
            {
                CurrentCombatant.UnHighlight();
            }
        }

        #endregion // MOUSEOVER =============


        #region HIGHLIGHTABLE
        // ==================================

        public void Highlight()
        {
            _isHighlighted = true;

            //print ($"{name} highlight");
        }

        public void Highlight(Color color)
        {
            _targetColor.Set(color);
            _customHighlight = true;

            //print($"{name} cust highlight");
        }

        public void UnHighlight()
        {
            _isHighlighted = false;
            _customHighlight = false;
            //print($"{name} UN");
        }

        public GameObject GameObject()
        {
            return gameObject;
        }

        #endregion // HIGHLIGHTABLE =========


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

            // Let the old tile know that we're gone
            if (combatant.CurrentTile != null)
            {
                combatant.CurrentTile.RemoveCombatant();
            }

            // Update new tile's current combatant
            _currentCombatant = combatant;
            combatant.CurrentTile = this;
        }

        public void RemoveCombatant()
        {
            _currentCombatant.CurrentTile = null;
            _currentCombatant = null;
        }
    }
}
