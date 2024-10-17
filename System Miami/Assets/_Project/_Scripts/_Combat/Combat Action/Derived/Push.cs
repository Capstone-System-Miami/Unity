// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "CombatAction/Push")]
    public class Push : CombatAction
    {
        [SerializeField] private int _distance;
        
        // TODO MapForward, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void Perform(Targets targets)
        {
            for (int i = 0; i < targets.Combatants.Length; i++)
            {
                if (targets.Combatants[i].TryGetComponent(out IMovable target))
                {
                    if (!target.TryMoveInDirection(_direction, _distance))
                    {
                        Debug.Log($"Target can't be pushed");
                    }
                    else
                    {
                        target.TryMoveInDirection(_direction, _distance);
                    }
                }
            }
        }

    }
}
