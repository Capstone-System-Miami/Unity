// Author: Layla, Lee, Johnny
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Restore Action", menuName = "Abilities/CombatActions/RestoreResource")]
    public class RestoreResource : CombatSubactionSO
    {
        [SerializeField] [Range(0, 1)] private float _percentage; // To change percentage to 50% or 75%, change the 0 to 0.5 or 0.75
        [SerializeField] private ResourceType resourceType;

        protected override ISubactionCommand GenerateCommand(ITargetable t)
        {
            throw new System.NotImplementedException();
        }

        //public void Perform()
        //{
        //    //foreach (Combatant target in TargetingPattern.StoredTargets.combatants)
        //    //{
        //    //    if (target == null) { continue; }

        //    //    switch (resourceType)
        //    //    {
        //    //        case ResourceType.Health:
        //    //            RestorePercentage(target.Health, _percentage); // Calculates Health
        //    //            break;
        //    //        case ResourceType.Stamina:
        //    //            RestorePercentage(target.Stamina, _percentage); // Calculates Stamina
        //    //            break;
        //    //        case ResourceType.Mana:
        //    //            RestorePercentage(target.Mana, _percentage); // Calculates Mana
        //    //            break;
        //    //        case ResourceType.Speed:
        //    //            RestorePercentage(target.Speed, _percentage); // Calculates Speed
        //    //            break;
        //    //        default:
        //    //            Debug.LogError("No valid ResourceType was selected.");
        //    //            break;
        //    //    }
        //    //}
        //}

        private void RestorePercentage(Resource resource, float percentage)
        {
            float restoreAmount = resource.GetMax() * percentage; // Calculate the percentage of max value
            resource.Gain(restoreAmount); // Restore the resource by 25% 
        }

        private enum ResourceType
        {
            Health,
            Stamina,
            Mana,
            Speed
        }
    }
}
