// Authors: Layla
using System;
using System.Collections.Generic;
using SystemMiami.CombatRefactor;
using UnityEngine;

namespace SystemMiami.ui
{
    public class CombatActionBar : MonoBehaviour
    {
        [SerializeField] private List<ActionQuickslot> quickslots;

        private int validSlotCount;

        private Type combatActionType;

        public void FillWith(List<CombatAction> combatActions)
        {
            validSlotCount = Math.Min(combatActions.Count, quickslots.Count);

            for (int i = 0; i < quickslots.Count; i++)
            {
                if (i >= validSlotCount)
                {
                    quickslots[i].DisableSelection();
                    continue;
                }

                quickslots[i].Fill(combatActions[i]);
            }
        }

        public void DisableAllExcept(ActionQuickslot slot)
        {
            foreach(ActionQuickslot quickslot in quickslots)
            {
                if (quickslot == slot)
                {
                    quickslot.Select();
                    continue;
                }

                quickslot.Deselect();
            }
        }
    }
}
