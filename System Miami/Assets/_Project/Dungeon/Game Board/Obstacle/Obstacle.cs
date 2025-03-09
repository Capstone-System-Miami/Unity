using System;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using SystemMiami.Management;
using SystemMiami.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

// Layla
namespace SystemMiami.CombatSystem
{
    public enum ObstacleType
    {
        STATIC_UNDAMAGEABLE,
        STATIC_DAMAGEABLE,
        DYNAMIC_UNDAMAGEABLE,
        DYNAMIC_DAMAGEABLE,
    }

    public abstract class Obstacle : MonoBehaviour, IHighlightable, ITargetable, ITileOccupant
    {
        [SerializeField] protected dbug log = new();

        private SpriteRenderer spriteRenderer;
        private ObstacleColorSetSO colorSet;
        private HighlightableStructSet<Color> untargetedColors = new();
        private HighlightableStructSet<Color> targetedColors = new();

        public ObstacleType ObstacleType { get; protected set; }

        [field: SerializeField, ReadOnly] public bool IsHighlighted { get; private set; }
        [field: SerializeField, ReadOnly] public bool IsTargeted { get; private set; }
        [field: SerializeField, ReadOnly] public bool IsLockedTarget { get; private set; }

        /// <summary>
        /// This is insanely confusing im so sorry I will fix it lol
        /// </summary>
        private Color CurrentColor => IsTargeted
            ? IsHighlighted
                ? IsLockedTarget
                    ? targetedColors.Highlighted
                    : targetedColors.Normal
                : targetedColors.Normal
            : IsHighlighted
                ? untargetedColors.Highlighted
                : untargetedColors.Normal;

        protected virtual void Awake()
        {
            if (ObstacleManager.MGR == null)
            {
                MapManager.MGR.AddComponent<ObstacleManager>();
            }
        }

        private void Start()
        {
            colorSet = ObstacleManager.MGR.GetColorSetByType(ObstacleType);
            Assert.IsNotNull(colorSet);
            untargetedColors = ValidateColorOpacity(colorSet.UntargetedColors);
            targetedColors = ValidateColorOpacity(colorSet.TargetedColors);
        }

        protected virtual void Update()
        {
            spriteRenderer.color = CurrentColor;
        }

        public void Initialize(Sprite sprite)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
            spriteRenderer.sortingLayerID = SortingLayer.NameToID("Player and Buildings - Decor");
            spriteRenderer.sortingOrder = 1;
        }


        #region ITileOccupant
        //============================================================
        [field: SerializeField, ReadOnly] public OverlayTile PositionTile { get; set; }

        public void SnapToPositionTile()
        {
            if (PositionTile == null)
            {
                Debug.LogError(
                    $"{gameObject}'s {this} tried to " +
                    $"snap to its posiiton tile, but " +
                    $"its PositionTile was null.");
            }
            transform.position = PositionTile.OccupiedPosition;
        }
        #endregion ITileOccupant


        #region IHighlightable
        //============================================================
        public void Highlight()
        {
            IsHighlighted = true;
        }

        public void Highlight(Color color)
        {
            IsHighlighted = true;
        }

        public void UnHighlight()
        {
            IsHighlighted = false;
        }

        GameObject IHighlightable.GameObject()
        {
            return gameObject;
        }
        #endregion IHighlightable


        #region ITargetable
        //============================================================
        List<ISubactionCommand> ITargetable.TargetedBy { get; set; } = new();
        public string nameMessageForDB { get { return gameObject.name; } set {; } }

        void ITargetable.SubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            Assert.IsNotNull(gameObject);
            log.warn($"inside {gameObject.name}'s Subscribe to action fn", this.gameObject);
            log.warn($"the event has {combatActionEvent.GetInvocationList().Length} subs)");

            combatActionEvent += HandleTargetingEvent;
            log.warn(
                $"after {gameObject}'s subsribtion the event has " +
                $"{combatActionEvent.GetInvocationList().Length} subs)");
        }

        void ITargetable.UnsubscribeTo(
            ref EventHandler<TargetingEventArgs> combatActionEvent)
        {
            IsTargeted = false;
            IsLockedTarget = false;
            combatActionEvent -= HandleTargetingEvent;
        }

        public void HandleTargetingEvent(object sender, TargetingEventArgs args)
        {
            log.print($"Trying to process a TargetingEvent of type {args.EventType}", gameObject);
            if (this is not ITargetable me) { return; }

            if (sender == null) return;

            switch (args.EventType)
            {

                case TargetingEventType.CANCELLED:
                    UnHighlight();
                    IsTargeted = false;
                    IsLockedTarget = false;
                    me.PreviewOff();
                    break;

                case TargetingEventType.STARTED:
                    Highlight();
                    IsTargeted = true;
                    IsLockedTarget = false;
                    break;

                case TargetingEventType.LOCKED:
                    IsTargeted = true;
                    IsLockedTarget = true;
                    me.PreviewOn();
                    break;

                case TargetingEventType.EXECUTING:
                    me.ApplyCombatAction();
                    break;

                case TargetingEventType.COMPLETED:
                    UnHighlight();
                    IsTargeted = false;
                    IsLockedTarget = false;
                    /// TODO: Wait until !TargetedBy.Any() ?
                    break;

                case TargetingEventType.REPORTBACK:
                    log.print("Im subbed.", this);
                    break;

                default:
                    break;
            }
        }

        public void PreviewOn()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction => subaction.Preview());
            log.print(
                $"{gameObject.name} wants to START" +
                $"displaying a preivew.");
        }

        public void PreviewOff()
        {
            if (this == null) return;
            log.print(
                $"{gameObject.name} wants to STOP" +
                $"displaying a preivew.");
        }

        void ITargetable.ApplyCombatAction()
        {
            if (this is not ITargetable me) { return; }

            me.TargetedBy.ForEach(subaction => subaction.Execute());
        }

        /// <inheritdoc />
        /// <remarks>
        /// Abstract in base class.
        /// Some obstacles will be damageable, others will not
        /// </remarks>
        public abstract IDamageReceiver GetDamageInterface();

        /// <inheritdoc />
        /// <remarks>
        /// Game board obstacles will never be healable
        /// </remarks>
        public IHealReceiver GetHealInterface()
        {
            return null;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Abstract in base class.
        /// Some obstacles will be movable, others will not
        /// </remarks>
        public abstract IForceMoveReceiver GetMoveInterface();

        /// <inheritdoc />
        /// <remarks>
        /// Game board obstacles will never have status effects
        /// </remarks>
        public IStatusEffectReceiver GetStatusEffectInterface()
        {
            return null;
        }
        #endregion ITargetable


        /// <summary>
        /// If the transparency of both colors is zero, then set both
        /// opacities to 1f (100%)
        /// </summary>
        /// <param name="colorSet"></param>
        /// <returns></returns>
        private HighlightableStructSet<Color> ValidateColorOpacity(
            HighlightableStructSet<Color> colorSet)
        {
            if (colorSet.Highlighted.a != 0 && colorSet.Normal.a != 0)
            {
                Debug.LogError("Returning original colors");
                return colorSet;
            }

            Color normal = new (
                colorSet.Normal.r,
                colorSet.Normal.g,
                colorSet.Normal.b,
                1f
            );

            Color highlight = new (
                colorSet.Highlighted.r,
                colorSet.Highlighted.g,
                colorSet.Highlighted.b,
                1f
            );

            Debug.LogError($"Returning new colors");
            return new HighlightableStructSet<Color>(normal, highlight);
        }
    }
}
