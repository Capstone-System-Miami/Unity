// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "Abilities/CombatActions/Push")]
    public class Push : CombatAction
    {
        [SerializeField] private int _distance;
        
        // TODO MapForwardA, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void Perform()
        {
            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target == null) { continue; }

                target.GetPushed(_distance, _direction);
            }
        }

    }
}
