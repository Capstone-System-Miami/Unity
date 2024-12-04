//Author: Lee
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        [Header("Item Properties")] 
        public string itemName;

        public Sprite icon;
        [TextArea] 
        public string description;

        [Header("Stacks")] 
        public bool isStackable = true;

        public int maxStackStize = 99;

        [Header("Combat Actions")]
        [SerializeField, Tooltip("If it is a consumable it uses the same combat actions as an ability.")]
        private CombatAction[] _actions;

        [HideInInspector] public Combatant user;

        /// <summary>
        /// Virtual method to define what happens when the item is used.
        /// Can be overridden by derived item classes.
        /// </summary>
        /// <param name="combatant">The combatant using the item.</param>
        public virtual void Use(Combatant combatant)
        {
            user = combatant;
            if (_actions != null)
            {
                foreach (CombatAction action in _actions)
                {
                    action.Perform();
                    action.userAction = user;
                }
            }
        }

       
    }
}
