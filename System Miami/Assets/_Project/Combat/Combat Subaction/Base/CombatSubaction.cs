// Authors: Layla Hoey, Lee St. Louis
using SystemMiami.CombatRefactor;
using SystemMiami.Utilities;
using System.Linq;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    /// An abstract class
    /// (Meaning an Object can't be ONLY a CombatAction.
    /// If an Object is of the class CombatAction,
    /// it has to be an Object of a class that inherits from CombatAction)
    public abstract class CombatSubaction : ScriptableObject
    {
        [Tooltip("Directions and distance this action will check for targets")]
        [SerializeField] private TargetingPattern _targetingPattern;

        public TargetingPattern TargetingPattern { get { return _targetingPattern; } }

        protected Targets currentTargets;

        public abstract void Perform();

        public void UpdateTargets(DirectionContext newUserDirection, bool directionChanged)
        {
            if (TargetingPattern.PatternOrigin
                == PatternOriginType.USER
                && !directionChanged)
            {
                return;
            }
            
            currentTargets?.all.ForEach(target => target.HandleEndTargeting(TargetingPattern.TargetedTileColor));

            currentTargets = _targetingPattern.GetTargets(newUserDirection);

            if (currentTargets == null){ Debug.LogWarning("fuck 1"); return; }
            if (!currentTargets.all.Any()){ Debug.LogWarning("fuck 2"); return; }
            if (currentTargets.all[0] == null) { Debug.LogWarning("fuck 3"); return; }
            currentTargets.all.ForEach(target => target.HandleBeginTargeting(TargetingPattern.TargetedTileColor));
        }

        public void ClearTargets()
        {
            currentTargets?.all.ForEach(target => target.HandleEndTargeting(TargetingPattern.TargetedTileColor));
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
