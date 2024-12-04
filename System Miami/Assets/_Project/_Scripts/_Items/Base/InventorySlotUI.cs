// Authors: Lee
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SystemMiami.CombatSystem;


namespace SystemMiami
{
    /// <summary>
    /// Represents a single inventory slot in the UI.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Components")]
        [SerializeField] private Image icon;             // Icon image component to display the item
        [SerializeField] private Text quantityText;      // Text component to display item quantity
        [SerializeField] private Image cooldownOverlay;  // Overlay image to display item cooldown
        [SerializeField] private Text name;  

        //[SerializeField] private TooltipUI tooltipUI;    // Reference to the tooltip UI

        [Header("Slot Information")]
        public int slotIndex;                            // Index of the slot in the inventory

        private InventorySlot _slot;                     // Reference to the inventory slot data

        [SerializeField] private Item itemInSlot;
        // Reference to the EquipmentManager for applying modifiers
       // private EquipmentManager _equipmentManager;

        // Reference to the player's Combatant component
        private Combatant _playerCombatant;

        private void Start()
        {
            // Get the EquipmentManager and Combatant from the player
            _playerCombatant = FindObjectOfType<PlayerController>().GetComponent<Combatant>();
            //_equipmentManager = _playerCombatant.GetComponent<EquipmentManager>();

            // Initialize UI components
           // ClearSlot();

            // Subscribe to inventory changes if needed
            _playerCombatant.Inventory.InventoryChanged += UpdateSlotUI;
        }

        /// <summary>
        /// Populates the slot UI with item data.
        /// </summary>
        /// <param name="slot">The inventory slot containing item data.</param>
        public void AddItem(InventorySlot slot)
        {
            _slot = slot;
            itemInSlot = slot.Item;
            // Update the icon to display the item's sprite
            
            
                icon.sprite = slot.Item.icon;
                
            

            if (slot.Item.name != null)
            {
                name.text = slot.Item.name;
            }
                // Display quantity if the item is stackable
           if (quantityText != null)
           {
                if (slot.Item.isStackable)
                {
                    quantityText.text = slot.Quantity.ToString();
                    quantityText.enabled = true;
                }
                else
                {
                    quantityText.enabled = false;
                }
           }

            // Update cooldown overlay if necessary
            if (cooldownOverlay != null)
            {
                cooldownOverlay.enabled = false;
            }
        }

        /// <summary>
        /// Clears the slot UI, removing any displayed item.
        /// </summary>
        public void ClearSlot()
        {
            _slot = null;

            if (icon != null)
            {
                icon.sprite = null;
               
            }

            if (quantityText != null)
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }

            if (cooldownOverlay != null)
            {
                cooldownOverlay.enabled = false;
            }
        }

        /// <summary>
        /// Updates the slot UI when the inventory changes.
        /// </summary>
        private void UpdateSlotUI()
        {
            if (_slot != null && !_slot.IsEmpty)
            {
                AddItem(_slot);
            }
            else
            {
                ClearSlot();
            }
        }

        public void OnClick()
        {

        }

        /// <summary>
        /// Handles pointer click events on the slot UI.
        /// </summary>
        /// <param name="eventData">Data associated with the pointer event.</param>
        public void OnPointerClick()
        {
            if (_slot != null && !_slot.IsEmpty)
            {
                // Check if the item is an EquipmentModifier
                if (_slot.Item is EquipmentModifier modifier)
                {
                    // Apply the modifier to the fixed equipment
                    EquipmentType targetType = modifier.targetEquipmentType;

                    // Apply the modifier to the equipment
                    //_equipmentManager.ApplyModifier(targetType, modifier);

                    // Remove the modifier item from the inventory
                    _playerCombatant.Inventory.RemoveItem(modifier, 1);

                    // Update the slot UI
                    UpdateSlotUI();
                }
                else
                {
                    // Use the item normally
                    _playerCombatant.Inventory.UseItem(slotIndex, _playerCombatant);

                    // Update the slot UI
                    UpdateSlotUI();
                }
            }
        }

        /// <summary>
        /// Handles pointer enter events to show item tooltip.
        /// </summary>
        /// <param name="eventData">Data associated with the pointer event.</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (_slot != null && !_slot.IsEmpty && tooltipUI != null)
            //{
            //   // tooltipUI.ShowTooltip(_slot.Item);
            //}
        }

        /// <summary>
        /// Handles pointer exit events to hide item tooltip.
        /// </summary>
        /// <param name="eventData">Data associated with the pointer event.</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            //if (tooltipUI != null)
            //{
            //    tooltipUI.HideTooltip();
            //}
        }

        /// <summary>
        /// Updates the cooldown overlay based on item cooldown.
        /// </summary>
        /// <param name="cooldown">The remaining cooldown time.</param>
        /// <param name="maxCooldown">The maximum cooldown time.</param>
        public void UpdateCooldown(float cooldown, float maxCooldown)
        {
            if (cooldownOverlay != null)
            {
                if (cooldown > 0)
                {
                    cooldownOverlay.enabled = true;
                    cooldownOverlay.fillAmount = cooldown / maxCooldown;
                }
                else
                {
                    cooldownOverlay.enabled = false;
                }
            }
        }
    }
}
