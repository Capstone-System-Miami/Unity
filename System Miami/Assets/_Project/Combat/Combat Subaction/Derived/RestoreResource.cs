using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New restore Action", menuName = "Abilities/CombatActions/RestoreResource")]
    public class RestoreResource : CombatSubaction
    {

        [SerializeField] private float _amount;
        [SerializeField] private ResourceType resourceType;
        public override void Perform()
        {
            foreach (Combatant target in TargetingPattern.StoredTargets.Combatants)
            {
                if (target == null) { continue; }

                switch (resourceType) 
                {
                    case ResourceType.Health:
                        target.RestoreResource(target.Health, _amount);
                        break;
                    case ResourceType.Stamina:
                        target.RestoreResource(target.Stamina, _amount);
                        break;
                    case ResourceType.Mana:
                        target.RestoreResource(target.Mana, _amount);
                        break;
                    case ResourceType.Speed:
                        target.RestoreResource(target.Speed, _amount);
                        break;
                    default:
                        Debug.Log("No ResourceType was selected");
                        break;
                }
                    
                 
                
            }
        }
    }
}
public enum ResourceType
{
    Health,
    Stamina,
    Mana,
    Speed
}
