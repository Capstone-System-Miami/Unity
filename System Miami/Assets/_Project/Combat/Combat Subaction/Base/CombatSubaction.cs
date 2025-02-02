// Authors: Layla Hoey, Lee St. Louis
using SystemMiami.CombatRefactor;
using SystemMiami.Utilities;
using System.Linq;
using UnityEngine;
using System;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatSubaction.
    /// If an Object is of the class CombatSubaction,
    /// it has to be an Object of a class that inherits from CombatSubaction)
    public abstract class CombatSubaction : ScriptableObject
    {
        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        protected Targets currentTargets;

        protected bool registered;

        public abstract void Perform();

        public void RegisterForActionUpdates(CombatAction combatAction)
        {
            Debug.LogWarning($"{this} trying to register for combatevents");

            combatAction.CombatActionEvent += HandleCombatActionEvent;
        }

        public void DeregisterForActionUpdates(CombatAction combatAction)
        {
            combatAction.CombatActionEvent -= HandleCombatActionEvent;
        }

        protected void HandleCombatActionEvent(object sender, CombatActionEventArgs args)
        {
            Debug.LogWarning($"{this} trying to handle a {args.actionState} event");

            switch (args.actionState)
            {
                default:
                case CombatActionEventType.UNEQUIPPED:
                    DeregisterForDirectionUpdates(args.user);
                    EndTargeting();
                    break;

                case CombatActionEventType.EQUIPPED:
                    RegisterForDirectionUpdates(args.user);
                    break;

                case CombatActionEventType.CONFIRMED:
                    break;

                case CombatActionEventType.EXECUTING:
                    break;

                case CombatActionEventType.COMPLETED:
                    break;
            }
        }

        protected void RegisterForDirectionUpdates(Combatant user)
        {
            Debug.LogWarning($"{this} trying to register for tile updates");

            if (registered) { return; }

            if (TargetingPattern.PatternOrigin == PatternOriginType.USER)
            {
                user.DirectionChanged += HandleDirectionChanged;
            }
            else
            {
                user.FocusTileChanged += HandleFocusTileChanged;
            }

            registered = true;
            Debug.LogWarning($"{this} registered for tile updates");
        }

        protected void DeregisterForDirectionUpdates(Combatant user)
        {
            if (!registered) { return; }

            if (TargetingPattern.PatternOrigin == PatternOriginType.USER)
            {
                user.DirectionChanged -= HandleDirectionChanged;
            }
            else
            {
                user.FocusTileChanged -= HandleFocusTileChanged;
            }

            registered = false;
        }

        protected void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            ClearTargets();
            UpdateTargets(args.directionContext);
            BeginTargeting();
        }

        protected void HandleDirectionChanged(
            object sender,
            DirectionChangedEventArgs args)
        {
            ClearTargets();
            UpdateTargets(args.newDirectionContext);
            BeginTargeting();
        }

        public void UpdateTargets(DirectionContext userDirection)
        {
            currentTargets = _targetingPattern.GetTargets(userDirection);
        }

        protected void BeginTargeting()
        {
            Debug.Log("Made it to begin targs");
            if (currentTargets == null) { Debug.LogWarning("fuck 1"); return; }
            if (!currentTargets.all.Any()) { Debug.LogWarning("fuck 2"); return; }
            if (currentTargets.all[0] == null) { Debug.LogWarning("fuck 3"); return; }

            currentTargets.all.ForEach(target => target.HandleBeginTargeting(TargetingPattern.TargetedTileColor));
        }

        protected void EndTargeting()
        {
            currentTargets?.all.ForEach(target => target.HandleEndTargeting(TargetingPattern.TargetedTileColor));
        }

        public void ClearTargets()
        {
            currentTargets?.all.ForEach(target => target.HandleEndTargeting(TargetingPattern.TargetedTileColor));
            currentTargets = null;
        }

        #region Private
        //private void showTiles()
        //{
        //    if (currentTargets.tiles == null) return;
        //    if (currentTargets.tiles.Count == 0) { return; }

        //    foreach (OverlayTile tile in currentTargets.tiles)
        //    {
        //        tile.Highlight(TargetedTileColor);
        //    }
        //}

        //private void showCombatants()
        //{
        //    if (currentTargets.tombatants == null) return;
        //    if (currentTargets.tombatants.Count == 0) { return; }

        //    for (int i = 0; i < currentTargets.tombatants.Count; i++)
        //    {
        //        if (currentTargets.tombatants[i] == null) { continue; }

        //        currentTargets.tombatants[i].Highlight(TargetedCombatantColor);
        //    }
        //}

        //private void hideTiles()
        //{
        //    if (currentTargets.tiles == null) return;
        //    if (currentTargets.tiles.Count == 0) { return; }

        //    foreach (OverlayTile tile in currentTargets.tiles)
        //    {
        //        tile.UnHighlight();
        //    }
        //}

        //private void hideCombatants()
        //{
        //    if (currentTargets.tombatants == null) return;
        //    if (currentTargets.tombatants.Count == 0) { return; }

        //    for (int i = 0; i < currentTargets.tombatants.Count; i++)
        //    {
        //        if (currentTargets.tombatants[i] == null) { continue; }

        //        currentTargets.tombatants[i].UnHighlight();
        //    }
        //}
        #endregion Private
    }
}
