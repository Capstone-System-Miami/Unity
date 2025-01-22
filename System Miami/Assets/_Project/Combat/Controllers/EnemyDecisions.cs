// Authors: Layla, Lee
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using UnityEngine;

namespace SystemMiami
{
    public class EnemyDecisions : CombatantDecisions
    {
        //[SerializeField] private int detectionRadius = 2;

        //private IEnumerator TurnBehavior()
        //{
        //    yield return new WaitForSeconds(.5f);

        //    // Movement
        //    FLAG_BeginMovement = true;

        //    yield return new WaitUntil(() => DestinationReached());
        //    yield return new WaitForSeconds(.5f);

        //    FLAG_NextPhase = true;
        //    yield return null;

        //    // Equip Ability
        //    // TODO use selectAbility() once that logic is figured out.
        //    TypeToEquip = AbilityType.PHYSICAL;
        //    IndexToEquip = 0;
        //    Debug.Log($"{name} starting equip cor.");
        //    yield return StartCoroutine(equipAbility());
        //    yield return null;

        //    // Can see player, check every direction rapidly.
        //    if (IsInDetectionRange(TurnManager.MGR.playerCharacter))
        //    {
        //        FocusedTile = TurnManager.MGR.playerCharacter.CurrentTile;
        //        yield return null;
        //        FocusedTileChanged?.Invoke(FocusedTile);

        //        for (int i = 0; i < System.Enum.GetValues(typeof(TileDir)).Length; i++)
        //        {
        //            FocusedTileChanged?.Invoke(FocusedTile);

        //            yield return new WaitForSeconds(.25f);

        //            // If the player is found in Ability's targets,
        //            // lock the ability and hold for a moment, then execute.
        //            if (combatant.Abilities.SelectedAbility.PlayerFoundInTargets)
        //            {
        //                yield return StartCoroutine(lockAbility());

        //                yield return new WaitForSeconds(.5f);

        //                yield return StartCoroutine(executeAbility());
        //                break;
        //            }

        //            // Turn Clockwise 45degrees
        //            AdjacentPositionSet adj = new AdjacentPositionSet(combatant.DirectionInfo);
        //            Vector2Int newMapPosToFace = adj.AdjacentPositions[(TileDir)1];

        //            if (MapManager.MGR.map.TryGetValue(newMapPosToFace, out OverlayTile newTileToFace))
        //            {
        //                FocusedTile = newTileToFace;
        //            }
        //        }
        //    }
        //    // Can't see player, but check some random directions just in case,
        //    // and for the visual element.
        //    else
        //    {
        //        float lookAroundDuration = 2f;
        //        float changeTargetInterval = .5f;
        //        while (lookAroundDuration > 0)
        //        {
        //            FocusedTile = MapManager.MGR.GetRandomUnblockedTile();                    
        //            FocusedTileChanged?.Invoke(FocusedTile);

        //            if (combatant.Abilities.SelectedAbility.PlayerFoundInTargets)
        //            {
        //                yield return StartCoroutine(lockAbility());

        //                yield return new WaitForSeconds(.5f);

        //                yield return StartCoroutine(executeAbility());
        //                break;
        //            }

        //            lookAroundDuration -= changeTargetInterval;
        //            yield return new WaitForSeconds(changeTargetInterval);
        //        }

        //    }

        //    yield return StartCoroutine(unequipAbility());

        //    yield return new WaitForSeconds(1f);
        //    FLAG_EndTurn = true;
        //}
        // ======================================



    }
}
