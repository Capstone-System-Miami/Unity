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
        [Header("Messages")]
        [SerializeField] private TextBox inputPromptPanel;


        [Header("Loadout UI")]
        [SerializeField] private CombatActionBar physicalAbilitiesBar;
        [SerializeField] private CombatActionBar magicalAbilitiesBar;
        [SerializeField] private CombatActionBar consumablesBar;

        public Action<ActionQuickslot> SlotClicked;
        public Action<Loadout, Combatant> CombatantLoadoutCreated;
        

       

        public void CreatePlayerLoadout(Combatant combatant)
        {
            OnCreatePlayerLoadout(combatant);
        }

        public void UpdateInputPrompt(string msg)
        {
            inputPromptPanel.ShowBackground();
            inputPromptPanel.ShowForeground();
            inputPromptPanel.SetForeground(msg);
        }

        public void ClearInputPrompt()
        {
            inputPromptPanel.SetForeground("");
            inputPromptPanel.HideBackground();
            inputPromptPanel.HideForeground();
        }

        public void ClickSlot(ActionQuickslot slot)
        {
            physicalAbilitiesBar.DisableAllExcept(slot);
            magicalAbilitiesBar.DisableAllExcept(slot);
            consumablesBar.DisableAllExcept(slot);

            // Raise event
            OnSlotClicked(slot);
        }

        protected virtual void OnCreatePlayerLoadout(Combatant combatant)
        {
            Loadout combatantLoadout = new(
                combatant._inventory,
                combatant);

            if (combatant is PlayerCombatant)
            {
                FillLoadoutBars(combatantLoadout);
            }

            CombatantLoadoutCreated.Invoke(combatantLoadout, combatant);
        }

        protected virtual void OnSlotClicked(ActionQuickslot slot)
        {
            SlotClicked?.Invoke(slot);
        }

        private void FillLoadoutBars(Loadout loadout)
        {
            physicalAbilitiesBar.FillWith(loadout.PhysicalAbilities.Cast<CombatAction>().ToList());
            magicalAbilitiesBar.FillWith(loadout.MagicalAbilities.Cast<CombatAction>().ToList());
            consumablesBar.FillWith(loadout.Consumables.Cast<CombatAction>().ToList());
        }
    }
}