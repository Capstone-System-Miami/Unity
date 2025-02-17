using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    /// <summary>
    /// A specialized Inventory for enemies, which never changes at runtime. 
    /// 
    /// Because it inherits from Inventory, all scripts that
    /// reference Inventory will work without modification.
    /// </summary>
    public class EnemyInventory : Inventory
    {
   
        /// <summary>
        /// Awake is called before any other script's Start().
        /// Here, we copy our Inspector-assigned lists into the base Inventory fields.
        /// </summary>
        protected override void Awake()
        {

            QuickslotMagicalAbilityIDs  .AddRange(MagicalAbilityIDs);
            QuickslotPhysicalAbilityIDs .AddRange(PhysicalAbilityIDs);
            QuickslotConsumableIDs      .AddRange(ConsumableIDs);
        }

       
    }
}
