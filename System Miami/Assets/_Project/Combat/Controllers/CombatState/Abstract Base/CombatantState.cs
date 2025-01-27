using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using System.Collections;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatantState
    {
        public readonly Phase phase;

        public readonly Combatant combatant;
        protected readonly CombatantStateFactory factory;
        protected readonly CombatantStateMachine machine;

        private string uiMsg = "";
        public string UI_Prompts
        {
            get
            {
                return uiMsg;
            }
            protected set
            {
                uiMsg = value;
                if (combatant.PrintUItoConsole)
                {
                    Debug.Log($"{combatant.name} UI message: {uiMsg}");
                }
            }
        }

        protected CombatantState(
            Combatant combatant,
            Phase phase
            )
        {
            this.combatant = combatant;
            this.factory = combatant.Factory;
            this.machine = combatant.StateMachine;
            this.phase = phase;
        }

        /// <summary>
        /// To be called ONCE,
        /// as soon as the state becomes active
        /// </summary>
        public virtual void OnEnter()
        {
            Debug.Log(
                $"{combatant.name} is entering" +
                $"a state: {this.GetType()}");
        }

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

        /// <summary>
        /// The main method by which state transitions happen.
        /// </summary>
        public void SwitchState(CombatantState newState)
        {
            /// Call OnExit on
            /// the current state object
            /// before setting a new one
            OnExit();

            /// Call OnEnter on the new state object
            newState.OnEnter();

            /// Set the current state to the new state.
            /// passed into this function as an arg.
            combatant.CurrentState = newState;
        }

        /// <summary>
        /// An overload of SwitchState that takes a float.
        /// This function will run a Coroutine that
        ///  then
        /// waits `float delay` seconds before assigning
        /// a new state and calling OnEnter() on it.
        /// </summary>
        /// 
        /// <param name="newState">
        /// The state to transition to.
        /// </param>
        /// 
        /// <param name="delay">
        /// Amount of time in seconds for
        /// the state machine to wait between: 
        /// (a) calling OnExit() on the previous state, and
        /// (b) calling OnEnter() on the new current state.
        /// </param>
        public void SwitchState(CombatantState newState, float delay)
        {
            combatant.StartCoroutine(SwitchStateWithDelay(newState, delay));
        }

        /// <summary>
        /// The IEnumerator by which the
        /// state machine can perform state changes
        /// with a delay.
        /// </summary>
        /// 
        /// <returns>
        /// The enumerated process
        /// </returns>
        private IEnumerator SwitchStateWithDelay(CombatantState newState, float delay)
        {
            OnExit();

            yield return new WaitForSeconds(delay);

            combatant.CurrentState = newState;
            newState.OnEnter();
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