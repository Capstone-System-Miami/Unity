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
        public override void SetTargeting()
        {
            Debug.Log($"{name} trying to set targets");

            _targeting = new Targeting(_user, this);
        }

        public override void Perform()
        {
            for(int i = 0; i < _targetCombatants.Length; i++)
            {
                if (!_targetCombatants[i].TryGetComponent(out IDamageable target))
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
