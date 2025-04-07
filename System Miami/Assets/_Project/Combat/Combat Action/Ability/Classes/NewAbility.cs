using System.Collections;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using System;

namespace SystemMiami.CombatRefactor
{
    public abstract class NewAbility : CombatAction
    {
        public readonly float ResourceCost;
        public readonly int CooldownTurns;

        private Action<float> looseResource;
        public int CooldownRemaining { get; private set; }

        public bool IsOnCooldown
        {
            get
            {
                return CooldownRemaining > 0;
            }
        }

        protected NewAbility(NewAbilitySO preset, Combatant user, Action<float> looseResource)
            : base(
                preset.Icon,
                preset.itemData.ID,
                preset.Actions.ToList(),
                preset.OverrideController,
                preset.FighterOverrideController,preset.MageOverrideController,
                preset.TankOverrideController, preset.RogueOverrideController,preset.isGeneralAbility,
                user)
        {
            this.ResourceCost = preset.ResourceCost;
            this.CooldownTurns = preset.CooldownTurns;
            this.looseResource = looseResource;
        }

        public void ReduceCooldown()
        {
            if (CooldownRemaining > 0)
            {
                CooldownRemaining--;
            }
        }

        protected override void PreExecution()
        {
            looseResource?.Invoke(ResourceCost);
        }

        protected override void PostExecution()
        {
            StartCooldown();
        }

        private void StartCooldown()
        {
            CooldownRemaining = CooldownTurns;
        }
    }
}