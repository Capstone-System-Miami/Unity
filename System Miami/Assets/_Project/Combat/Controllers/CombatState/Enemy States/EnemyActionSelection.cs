using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using UnityEditor;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionSelection : ActionSelection
    {
        public EnemyActionSelection(Combatant combatant)
            : base (combatant) { }

        // TODO
        // Actually set their selected ability somewhere

        protected override bool EquipRequested()
        {
            // immediately to equipped?
            return true;
        }
        protected override bool SkipPhaseRequested()
        {
            // if they have to?
            return false;
        }
    }


    // VVV PLEASE KEEP FOR REFERENCE VVV
    // VVV PLEASE KEEP FOR REFERENCE VVV

    //#region Focused Tile
    //// ======================================

    //public override OverlayTile GetFocusedTile()
    //{

    //    Combatant targetPlayer = TurnManager.MGR.playerCharacter;

    //    if (IsInDetectionRange(targetPlayer))
    //    {
    //        Debug.Log($"Player found in {name}'s range");
    //        return targetPlayer.CurrentTile;
    //    }
    //    else
    //    {
    //        Debug.Log($"Player not found in {name}'s range." +
    //            $"Getting random tile");
    //        return MapManager.MGR.GetRandomUnblockedTile();
    //    }
    //}
    //// ======================================
    //#endregion // Focused Tile ==============


    //#region Movement

    ///// <summary>
    ///// Gets walkable neighbor tiles for random movement.
    ///// </summary>
    //private List<OverlayTile> GetWalkableNeighbourTiles()
    //{
    //    List<OverlayTile> walkableTiles = new List<OverlayTile>();

    //    List<OverlayTile> neighbours = PathFinder.GetNeighbourTiles(combatant.CurrentTile);

    //    foreach (OverlayTile tile in neighbours)
    //    {
    //        if (tile.ValidForPlacement)
    //        {
    //            walkableTiles.Add(tile);
    //        }
    //    }

    //    return walkableTiles;
    //}
    //#endregion


    //#region Abilities
    //private IEnumerator equipAbility()
    //{
    //    FLAG_Equip = true;
    //    Debug.Log($"{name} waiting for equip");
    //    yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.EQUIPPED);
    //}

    //private IEnumerator unequipAbility()
    //{
    //    FLAG_Unequip = true;
    //    Debug.Log($"{name} waiting for unequip");
    //    yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.UNEQUIPPED);
    //}

    //private IEnumerator lockAbility()
    //{
    //    FLAG_LockTargets = true;
    //    Debug.Log($"{name} waiting for targ lock");
    //    yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.TARGETS_LOCKED);
    //}

    //private IEnumerator executeAbility()
    //{
    //    FLAG_UseAbility = true;
    //    Debug.Log($"{name} waiting for execute");
    //    yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.EXECUTING);

    //    Debug.Log($"{name} waiting for complete");
    //    yield return new WaitUntil(() => combatant.Abilities.CurrentState == Abilities.State.COMPLETE);
    //}
    //#endregion
}
