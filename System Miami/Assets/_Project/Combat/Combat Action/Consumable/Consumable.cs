using System.Collections;
using System;
using SystemMiami.CombatSystem;
using System.Linq;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class Consumable : CombatAction
    {
        public event Action<Consumable> Consumed = delegate{ };

        public readonly int MaxUses;

        public int UsesRemaining { get; private set; }

        public bool IsEmpty
        {
            get
            {
                return UsesRemaining <= 0;
            }
        }

        public Consumable(ConsumableSO preset, Combatant user)
            : base(
                  preset.Icon,
                  preset.itemData.ID,
                  preset.Actions.ToList(),
                  preset.OverrideController,
                  user)
        {
            MaxUses = (int)Mathf.Clamp(preset.Uses, 1, Mathf.Infinity);
            UsesRemaining = MaxUses;
        }

        protected override void PreExecution()
        {
            UsesRemaining--;
        }

        protected override void PostExecution()
        {
            if (UsesRemaining <= 0)
            {
                Consumed?.Invoke(this);
            }
        }
    }
}
