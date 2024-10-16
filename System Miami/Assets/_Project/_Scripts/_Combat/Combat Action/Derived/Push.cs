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
        
        // TODO Forward, backward, etc.
        // In reference to attacker or reciever though, idk.
        [SerializeField] private Vector2Int _direction;

        public override void SetTargeting()
        {
            _targeting = new Targeting(_user, this);
        }

        public override void Perform()
        {
            Combatant[] validEnemies = _targetCombatants;

            for (int i = 0; i < validEnemies.Length; i++)
            {
                if (validEnemies[i].TryGetComponent(out IMovable target))
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
