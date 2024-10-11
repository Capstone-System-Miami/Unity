// Authors: Layla Hoey
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

        public override void PerformOn(Combatant[] targets)
        {
            // TODO
            // Somehow calculate this?
            Vector2Int directionVec = Vector2Int.zero;

            foreach (Combatant target in targets)
            {
                target.GetPushed(_distance, directionVec);
            }
        }
    }
}
