using System.Collections;
using System;
using SystemMiami.CombatSystem;
using System.Linq;

namespace SystemMiami.CombatRefactor
{
    public class Consumable : CombatAction
    {
        public event Action<Consumable> Consumed = delegate{ };

        public readonly int MaxUses;

        private int usesRemaining;

        public int UsesRemaining
        {
            get
            {
                return usesRemaining;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return usesRemaining <= 0;
            }
        }

        public Consumable(ConsumableSO preset, Combatant user)
            : base(
                  preset.Icon,
                  preset.Actions.ToList(),
                  preset.OverrideController,
                  user)
        {
            MaxUses = preset.Uses;
        }

        protected override void PreExecution()
        {
            usesRemaining--;
        }

        protected override void PostExecution()
        {
            if (usesRemaining <= 0)
            {
                Consumed?.Invoke(this);
            }
        }
    }
}
