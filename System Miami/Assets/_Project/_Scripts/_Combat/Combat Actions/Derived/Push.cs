// Authors: Layla Hoey
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Push Action", menuName = "CombatAction/Push")]
    public class Push : CombatAction
    {
        [SerializeField] private int _distance;
        
        // TODO Forward, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void PerformOn(GameObject target)
        {
            // TODO
            // Somehow calculate this?
            Vector2Int directionVec = Vector2Int.zero;

            if (target.TryGetComponent(out IMovable movable))
            {
                movable.TryMoveTo(new Vector3(100, 100, 100));
            }
        }
    }
}
