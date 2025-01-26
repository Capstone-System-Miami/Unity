using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatantState
    {
        public readonly Phase Phase;

        protected readonly Combatant combatant;
        protected readonly CombatantStateMachine machine;

        protected CombatantState(
            Combatant combatant,
            Phase phase
            )
        {
            this.combatant = combatant;
            this.machine = combatant.StateMachine;
            Phase = phase;
        }

        /// <summary>
        /// To be called ONCE,
        /// as soon as the state becomes active
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// To be called EVERY FRAME while the state is active
        /// Anything that needs to be checked or adjusted
        /// That doesn't have to do with transitioning
        /// to the next state should be implemented here.
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// To be called EVERY FRAME after Update().
        /// This is where decisions should be made about
        /// whether to remain in this state,
        /// or transition into a new state
        /// (and if so, which new state? etc.)
        /// </summary>
        public virtual void MakeDecision() { }

        public virtual void LateUpdate() { }

        /// <summary>
        /// Called once, inside TransitionTo(newState),
        /// just before the state becomes inactive
        /// </summary>
        public virtual void OnExit() { }

        public virtual string rGetTurnPrompts()
        {
            return "";
        }

        // VVVVV KEEP FOR REFERENCE PLEASE  VVVVV

        //private string getMovementPrompt()
        //{
        //    string result = "";

        //    if (turnOwner.StateMachine.CanMove)
        //    {
        //        result += $"Click a tile to move,\n\n";
        //    }
        //    else if (turnOwner.Speed.Get() > 0)
        //    {
        //        result += $"Moving To Tile.\n\n";
        //    }
        //    else
        //    {
        //        result += $"Speed Depleted\n\n";
        //    }

        //    result += $"Press E to end Movement Phase\n\n" +
        //                $"Or Press Q to end Turn.";

        //    return result;
        //}

        //private string getActionPrompt()
        //{
        //    string result = "";

        //    if (turnOwner.StateMachine.CanAct)
        //    {
        //        result += $"Click an Ability to Equip it,\n\n" +
        //            $"Or ";
        //    }

        //    result += $"Press Q to End Turn.";

        //    return result;
        //}


        //#region Ability Responses
        //private void onEquipAbility(Ability ability)
        //{
        //    _overrideActionPrompt = true;
        //    _actionText = $"Left Click a Tile to Lock Targets";
        //}

        //private void onUnequipAbility()
        //{
        //    _overrideActionPrompt = false;
        //}

        //private void onLockedTargets(Ability ability)
        //{
        //    _overrideActionPrompt = true;
        //    _actionText = $"Press Enter to Use {ability.name}";
        //}

        //private void onUseAbility(Ability ability)
        //{
        //    _overrideActionPrompt = true;
        //    _actionText = $"Using {ability.name}";
        //}
        //#endregion
    }
}