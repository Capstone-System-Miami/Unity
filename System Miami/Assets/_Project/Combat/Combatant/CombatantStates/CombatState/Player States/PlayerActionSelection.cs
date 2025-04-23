using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class PlayerActionSelection : ActionSelection
    {
        bool slotBeenClicked = false;

        public PlayerActionSelection(Combatant combatant)
            : base(combatant) { }

        public override void OnEnter()
        {
            base.OnEnter();

            /// Update prompts
            InputPrompts =
                $"Select an action from your Loadout Menu.\n\n" +
                $"Or press {combatant.flowKey} to skip Actions and end your turn.";

            UI.MGR.UpdateInputPrompt(InputPrompts);

            UI.MGR.SlotClicked += HandleSlotClick;
        }

        public override void Update()
        {
            // For the player, we update the focus every frame.
            // Setting this as !canBeNull, so that when the
            // player mouses over towards their abilities,
            // the character focuses on the tile in front of them.
            combatant.UpdateFocus();
        }

        /// <summary>
        /// TODO: Test & ensure that this actually determines
        /// the correct CombatAction to
        /// return based on the input.
        /// </summary>
        /// <param name="slot"></param>
        private void HandleSlotClick(ActionQuickslot slot)
        {
            selectedCombatAction = slot.CombatAction;

            slotBeenClicked = true;
        }

        protected override bool EquipRequested()
        {
            if (slotBeenClicked)
            {
                slotBeenClicked = false;
                return true;
            }

            return false;
        }

        protected override bool SkipPhaseRequested()
        {
            return Input.GetKeyDown(combatant.flowKey);
        }
    }
}
