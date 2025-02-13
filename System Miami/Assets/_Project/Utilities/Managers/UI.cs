// Authors: Layla
using System;
using System.Linq;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using System.Collections.Generic;
using SystemMiami.ui;
using UnityEngine;

namespace SystemMiami.Management
{
    public class UI : Singleton<UI>
    {
        public Action<ActionQuickslot> SlotClicked;

        [SerializeField] private CombatActionBar physicalAbilitiesBar;
        [SerializeField] private CombatActionBar magicalAbilitiesBar;
        [SerializeField] private CombatActionBar consumablesBar;
        [SerializeField] private DatabaseSO database;
        public void ClickSlot(ActionQuickslot slot)
        {
            physicalAbilitiesBar.DisableAllExcept(slot);
            magicalAbilitiesBar.DisableAllExcept(slot);
            consumablesBar.DisableAllExcept(slot);

            // Raise event
            OnSlotClicked(slot);
        }

       
        protected virtual void OnSlotClicked(ActionQuickslot slot)
        {
            SlotClicked.Invoke(slot);
        }

        private void FillLoadoutBars(Loadout loadout)
        {
            List<CombatAction> convertedPhys
                = loadout.PhysicalAbilities.Select(
                    ability => (CombatAction)ability).ToList();

            List<CombatAction> convertedMagical
                = loadout.MagicalAbilities.Select(
                    ability => (CombatAction)ability).ToList();

            List<CombatAction> convertedConsumables
                = loadout.Consumables.Select(
                    ability => (CombatAction)ability).ToList();

            physicalAbilitiesBar.FillWith(convertedPhys);
            magicalAbilitiesBar.FillWith(convertedMagical);
            consumablesBar.FillWith(convertedConsumables);
        }
    }
}
