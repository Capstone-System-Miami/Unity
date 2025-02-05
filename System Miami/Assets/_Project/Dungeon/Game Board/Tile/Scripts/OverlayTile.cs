// Author: Alec, Layla
using System;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, IHighlightable, ITargetable
    {
        private readonly Vector3 offsetVector = new(0f, 0.0001f, 0f);

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

        private ITileOccupant occupant;
        private Vector3 occupantPosition;

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
        public Vector3Int GridLocation
            { get { return _gridLocation; } set { _gridLocation = value; } }

        public Vector3 OccupiedPosition
            { get { return transform.position + offsetVector; } }

        /// <summary>
        /// Reference to the combatant currently occupying this tile
        /// This is a getter property, and casts the ITileOccupier
        /// </summary>
        public Combatant CurrentCombatant
            { get { return occupant as Combatant; } }

        /// <summary>
        /// Whether or not the tile is occupied by a combatant.
        /// </summary>
        public bool Occupied
            { get { return occupant != null; } }

        public ITileOccupant Occupier
            { get { return occupant; } }

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
            { get { return !Occupied; } }

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
        public void SetOccupant(ITileOccupant newOccupant)
        {
            if (!ValidForPlacement) { return; }

            occupant = newOccupant;
            occupant.PositionTile = this;
            occupant.SnapToPositionTile();
        }

        public void ClearOccupant()
        {
            if (occupant == null) { return; }

            occupant.PositionTile = null;
            occupant = null;
        }

        public List<ISubactionCommand> TargetedBy { get; set; } = new();
        public string nameMessageForDB { get { return gameObject.name; } set { ; } }
        public void SubscribeTo(EventHandler<CombatActionEventArgs> combatActionEvent)
        {
            combatActionEvent += HandleCombatActionEvent;
        }

        public void UnsubscribeTo(EventHandler<CombatActionEventArgs> combatActionEvent)
        {
            combatActionEvent -= HandleCombatActionEvent;
        }

        public void HandleCombatActionEvent(object sender, CombatActionEventArgs args)
        {
            switch (args.eventType)
            {
                case CombatActionEventType.UNEQUIPPED:
                    TargetedBy.Clear();
                    UnHighlight();
                    break;
                case CombatActionEventType.EQUIPPED:
                    Highlight(Color.red);
                    break;
                case CombatActionEventType.CONFIRMED:
                    DisplayPreview();
                    break;
                case CombatActionEventType.EXECUTING:
                    ApplyCombatAction();
                    break;
                case CombatActionEventType.COMPLETED:
                    break;
                default:
                    break;
            }
        }

        public void DisplayPreview()
        {
            ///
        }

        public void ApplyCombatAction()
        {
            ///
        }

        public virtual bool TryGetDamageInterface(out IDamageReciever damageInterface)
        {
            damageInterface = null;
            return false;
        }

        public virtual bool TryGetHealInterface(out IHealReciever healInterface)
        {
            healInterface = null;
            return false;
        }

        public virtual bool TryGetMoveInterface(out IForceMoveReciever forceMoveInterface)
        {
            forceMoveInterface = null;
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
