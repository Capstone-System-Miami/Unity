// Authors: Lee
using UnityEngine;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(fileName = "New Restore Resource Action", menuName = "Combat Actions/Restore Resource Action")]
    public class RestoreResourceAction : CombatAction
    {
        [Header("Resource Restoration Settings")]

        [Tooltip("The type of resource to restore (e.g., Health, Mana, Stamina).")]
        public ResourceType resourceType;

        [Tooltip("The amount of resource to restore.")]
        public float restoreAmount;

        [Tooltip("If true, the restoreAmount is treated as a percentage of the target's maximum resource.")]
        public bool isPercentage;

        public override void Perform()
        {
           RestoreResource(userAction);
         
        }

        /// <summary>
        /// Restores the specified resource.
        /// </summary>
        /// <param name="combatant">The combatant to restore the resource to.</param>
        private void RestoreResource(Combatant combatant)
        {
            float amountToRestore = restoreAmount;

            // Calculate the amount if it's a percentage
            if (isPercentage)
            {
                float maxResource = GetMaxResource(combatant);
                amountToRestore = maxResource * (restoreAmount / 100f);
            }

            // Apply the restoration
            switch (resourceType)
            {
                case ResourceType.Health:
                    combatant.Health.Gain(amountToRestore);
                    Debug.Log($"{combatant.name} restored {amountToRestore} Health.");
                    break;

                case ResourceType.Mana:
                    combatant.Mana.Gain(amountToRestore);
                    Debug.Log($"{combatant.name} restored {amountToRestore} Mana.");
                    break;

                case ResourceType.Stamina:
                    combatant.Stamina.Gain(amountToRestore);
                    Debug.Log($"{combatant.name} restored {amountToRestore} Stamina.");
                    break;

                // Add other resource types as needed
                default:
                    Debug.LogWarning("RestoreResourceAction: Unsupported resource type.");
                    break;
            }
        }

        /// <summary>
        /// Gets the maximum value of the specified resource for the combatant.
        /// </summary>
        /// <param name="combatant">The combatant to get the max resource from.</param>
        /// <returns>The maximum resource value.</returns>
        private float GetMaxResource(Combatant combatant)
        {
            switch (resourceType)
            {
                case ResourceType.Health:
                    return combatant._stats.GetStat(StatType.MAX_HEALTH);

                case ResourceType.Mana:
                    return combatant._stats.GetStat(StatType.MANA);

                case ResourceType.Stamina:
                    return combatant._stats.GetStat(StatType.STAMINA);

                
                default:
                    return 0f;
            }
            
        }
    }

    
    public enum ResourceType
    {
        Health,
        Mana,
        Stamina
    }
}
