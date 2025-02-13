using System.Collections.Generic;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class EnemyActionSelection : ActionSelection
    {
        private const float DELAY = 0.25f;
        private CountdownTimer delayTimer;

        public EnemyActionSelection(Combatant combatant)
            : base (combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            delayTimer = new(combatant, DELAY);
            delayTimer.Start();
        }

        /// <summary>
        /// This method always returns true,
        /// and sets the selected combataction
        /// that will be sent to the equipped state.
        /// <para>
        /// NOTE that random selection means a
        /// different combatAction will be set every time
        /// this method runs.
        /// </para>
        /// <para>
        /// TODO consider a better solution later.
        /// </para>
        /// </summary>
        /// <returns></returns>
        protected override bool EquipRequested()
        {
            if (!delayTimer.IsFinished) { return false; }

            selectedCombatAction = SelectRandomCombatAction();
            Debug.Log(
                $"selecting combatAction, it is " +
                $"{( (selectedCombatAction != null) ? "not" : "" )}" +
                $"null", combatant);

            return true;
        }

        protected override bool SkipPhaseRequested()
        {
            return false;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        protected CombatAction SelectRandomCombatAction()
        {
            List<CombatAction> allCombatActions = new(combatant.Loadout.PhysicalAbilities);
            allCombatActions.AddRange(combatant.Loadout.MagicalAbilities);
            allCombatActions.AddRange(combatant.Loadout.Consumables);
            
            int randomIndex = Random.Range(0, allCombatActions.Count);

            /// TODO:
            /// determine a safe way to do this.
            if (allCombatActions == null) { return null; }
            if (!allCombatActions.Any()) { return null; }

            return allCombatActions[randomIndex];
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
