using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami.ui
{
    public class InventoryTabGroup : SingleSelectGroup<InventoryTab>
    {
        [SerializeField] private InventoryTab tabPhysical;
        [SerializeField] private InventoryTab tabMagical;
        [SerializeField] private InventoryTab tabConsumable;
        [SerializeField] private InventoryTab tabEquipment;

        private bool tabsInitialized;

        public InventoryTab TabPhysical { get { return tabPhysical; } }
        public InventoryTab TabMagical { get { return tabMagical; } }
        public InventoryTab TabConsumable { get { return tabConsumable; } }
        public InventoryTab TabEquipment { get { return tabEquipment; } }

        protected override List<InventoryTab> GetSelectables()
        {
            if (!tabsInitialized)
            {
                InitTabs();
            }

            return new()
            {
                TabPhysical,
                TabMagical,
                TabConsumable,
                TabEquipment,
            };
        }

        private void InitTabs()
        {
            tabPhysical.Initialize(ItemType.PhysicalAbility);
            tabMagical.Initialize(ItemType.MagicalAbility);
            tabConsumable.Initialize(ItemType.Consumable);
            tabEquipment.Initialize(ItemType.EquipmentMod);

            tabsInitialized = true;
        }
    }
}
