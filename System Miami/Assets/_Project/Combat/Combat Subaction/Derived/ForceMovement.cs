// Authors: Layla Hoey
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "Abilities/CombatActions/Push")]
    public class ForceMovement : CombatSubaction
    {
        [SerializeField] private int _distance;
        
        // TODO MapForwardA, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void Perform()
        {
            List<IMovable> targets = currentTargets.GetTargetsWith<IMovable>();

            foreach (IMovable target in targets)
            {
                if (target == null) { continue; }

                target.TryMoveInDirection(_direction, _distance);
            }
        }

    }
}
