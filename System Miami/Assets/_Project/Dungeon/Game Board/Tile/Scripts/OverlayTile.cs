// Author: Alec, Layla
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, IHighlightable, ITargetable
    {
        #region PUBLIC VARS
        // ==================================

        /// <summary>
        /// TODO ???
        /// </summary>
        public int G;

        /// <summary>
        /// TODO ???
        /// </summary>
        public int H;

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

        private ITargetable occupier;

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
        /// This is a getter property, and casts the ITileOccupier
        /// </summary>
        public Combatant CurrentCombatant { get { return occupier as Combatant; } }

        /// <summary>
        /// Whether or not the tile is occupied by a combatant.
        /// </summary>
        public bool Occupied
        {
            get
            {
                return occupier != null;
            }
        }

        public ITargetable Occupier
        {
            get
            {
                return occupier;
            }
        }

        /// <summary>
        /// Whether something can be positioned onto the tile.
        /// 
        /// TODO
        /// right now, this is just the opposite of
        /// <see cref="Occupied"/>.
        /// I'm not sure if we'll need to implement
        /// logic for "holes"?? A hole isn't quite an
        /// "Occupier", but it would definitely mean you
        /// can't move there. So I'm leaving this property
        /// here in case we need to add a condition
        /// like <c>IsGround</c> or something.
        /// </summary>
        public virtual bool ValidForPlacement
        {
            get
            {
                return !Occupied;
            }
        }

        /// <summary>
        /// TODO ???
        /// </summary>
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


        #region Visibility
        // ==================================



        #endregion // Visibility====


        #region Mouseover
        // ==================================
        public void BeginHover(Combatant combatant)
        {
            if (combatant.gameObject != PlayerManager.MGR.gameObject) { return; }

            if (Occupied)
            {
                Highlight();
                CurrentCombatant.Highlight();
            }

            Highlight();
        }

        public void EndHover(Combatant combatant)
        {
            if (combatant.gameObject != PlayerManager.MGR.gameObject) { return; }

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

        #endregion Mouseover


        #region IHighlightable
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

        #endregion IHighlightable


        /// <summary>
        /// Positions a combatant on this tile.
        /// If the combatant already has a CurrentTile,
        /// this function calls RemoveCombatant() on
        /// combatant's CurrentTile before setting it to
        /// this tile.
        /// </summary>
        public bool TryAddOccupier(ITargetable occupier)
        {
            if (!ValidForPlacement
                && this.occupier != occupier)
            {
                Debug.LogError(
                    $"Trying to place {occupier}" +
                    $"on{gameObject}." +
                    $"This placement is invalid."
                    );
                return false;
            }

            if (occupier is not Transform) { return false; }

            (occupier as Transform).position = new Vector3(transform.position.x, transform.position.y + 0.0001f, transform.position.z);

            this.occupier = occupier;

            return true;
        }

        public void RemoveOccupier(ITargetable occupier)
        {
            if (this.occupier != occupier) { return; }

            this.occupier = null;
        }

        public void HandleBeginTargeting(Color preferredColor)
        {
            Highlight(preferredColor);
        }

        public void HandleEndTargeting(Color preferredColor)
        {
            UnHighlight();
        }

        public bool TryGetDamageable(out IDamageReciever damageInterface)
        {
            damageInterface = null;
            return false;
        }

        public bool TryGetHealable(out IHealReciever healInterface)
        {
            healInterface = null;
            return false;
        }

        public bool TryGetMovable(out IForceMoveReciever moveInterface)
        {
            moveInterface = null;
            return false;
        }


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
                if (CurrentCombatant.IsMyTurn)
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
    }
}
