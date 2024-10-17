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
            for(int i = 0; i < targets.Combatants.Length; i++)
            {
                if (!targets.Combatants[i].TryGetComponent(out IDamageable target))
                {
                    Debug.Log("Invalid damage target");
                }
                else
                {
                    target.Damage(_abilityDamage);
                }
            }
        }
    }

}
