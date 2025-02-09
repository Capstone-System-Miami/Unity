// Author: Alec, Layla
using System;
using System.Collections.Generic;
using System.Linq;
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

        private object eventLock = new object();
        
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
            //if (combatant.gameObject != PlayerManager.MGR.gameObject) { return; }

            Highlight();

            if (TargetedBy.Any())
            {
                PreviewOn();
            }
        }

        public void EndHover(Combatant combatant)
        {
            //if (combatant.gameObject != PlayerManager.MGR.gameObject) { return; }

            //if (_customHighlight)
            //{
            //    _targetColor.Revert();
            //}
            //else

            UnHighlight();
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

            if (color == Color.blue) { Debug.LogError("Blue boi");}
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
        public void SubscribeTo(ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            Debug.LogWarning(
                $"inside {gameObject}'s SUBSCRIBE to action fn\n" +
                $"Pre subscription, sp assume Invocation list len is 0.");

            combatActionEvent += HandleTargetingEvent;


            Debug.LogWarning(
                $"inside {gameObject}'s SUBSCRIBE to action fn\n" +
                $"Invocation list len: {combatActionEvent?.GetInvocationList().Length}");
        }

        public void UnsubscribeTo(ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            Debug.LogWarning(
                $"inside {gameObject}'s UNSUBSCRIBE to action fn\n" +
                $"Pre subscription, sp assume Invocation list len is 0.");

            combatActionEvent -= HandleTargetingEvent;

            Debug.LogWarning(
                $"inside {gameObject}'s UNSUBSCRIBE to action fn\n" +
                $"Invocation list len: {combatActionEvent?.GetInvocationList().Length}");
        }

        public void HandleTargetingEvent(object sender, TargetingEventArgs args)
        {
            Debug.LogWarning($"Handling Targeting Event of type {args.EventType}");
            switch (args.EventType)
            {
                case TargetingEventType.CANCELLED:
                    UnHighlight();
                    break;

                case TargetingEventType.STARTED:
                    Highlight(Color.yellow + new Color(0, -0.2f, 0, 0));                    
                    break;

                case TargetingEventType.LOCKED:
                    Highlight(Color.red);
                    PreviewOn();
                    break;
                case TargetingEventType.EXECUTING:
                    ApplyCombatAction();
                    break;
                case TargetingEventType.COMPLETED:

                    break;
                default:
                    break;
            }
        }

        public void PreviewOn()
        {
            Debug.Log(
                $"{gameObject.name} wants to START" +
                $"displaying a preivew.");
        }

        public void PreviewOff()
        {
            Debug.Log(
                $"{gameObject.name} wants to STOP" +
                $"displaying a preivew.");
        }

        public void ApplyCombatAction()
        {
            Debug.Log(
                $"{gameObject.name} wants to get some" +
                $"subactions done to itself.");
        }

        public virtual IDamageReceiver GetDamageInterface()
        {
            return null;
        }

        public virtual IHealReceiver GetHealInterface()
        {
            return null;
        }

        public virtual IForceMoveReceiver GetMoveInterface()
        {
            return null;
        }

        public virtual IStatusEffectReceiver GetStatusEffectInterface()
        {
            return null;
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
