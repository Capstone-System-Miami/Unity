// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(fileName = "New Projectile Action", menuName = "CombatAction/Projectile")]
    public class Projectile : CombatAction
    {
        [SerializeField] int _range;

        public int Range { get { return _range; } }

        public override void Perform(Targets targets)
        {
            for(int i = 0; i < targets.Combatants.Length; i++)
            {
                if (!targets.Combatants[i].TryGetComponent(out Combatant target))
                {
                    Debug.Log($"{name} didn't find anything in its combatant array at {i}");
                }
                else
                {
                    // don't know how this would work yet.
                    Debug.Log($"{name} would be doin some stuff right now.");
                }
            }
        }
    }
}
