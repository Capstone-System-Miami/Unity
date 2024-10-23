using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Debug Action", menuName = "Abilities/CombatActions/Debug")]
    public class DebugCombatAction : CombatAction
    {
        
        public override void Perform()
        {           
            Debug.Log("Ability has been used!");
        }
    }
}

    
