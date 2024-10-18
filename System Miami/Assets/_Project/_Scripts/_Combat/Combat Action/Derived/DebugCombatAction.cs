using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Debug Action", menuName = "CombatAction/Debug")]
    public class DebugCombatAction : CombatAction
    {
        
        public override void Perform(Targets target)
        {
           
            Debug.Log("Ability has been used!");
        }
    }
}

    
