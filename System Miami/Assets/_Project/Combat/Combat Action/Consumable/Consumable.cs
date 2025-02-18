using System.Collections;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public class Consumable : CombatAction
    {
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
                  preset.Actions,
                  preset.OverrideController,
                  user)
        {
            MaxUses = preset.Uses;
        }


        public override IEnumerator Use()
        {
            performActions();

            yield return null;

            usesRemaining--;
        }
    }
}
