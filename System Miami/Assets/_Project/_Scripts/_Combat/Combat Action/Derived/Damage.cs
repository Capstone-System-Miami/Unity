// Authors: Layla Hoey
using UnityEngine;
using SystemMiami.AbilitySystem;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.PackageManager;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Damage Action", menuName = "CombatAction/Damage")]
    public class Damage : CombatAction
    {
        [SerializeField] private float _abilityDamage;

        public override void Perform(Targets targets)
        {
            // Loop through each combatant in the targets and apply damage.
            foreach (Combatant targetCombatant in targets.Combatants)
            {
                if (targetCombatant != null)
                {
                    targetCombatant.Damage(_abilityDamage);
                }
            }
        }
    }

}
