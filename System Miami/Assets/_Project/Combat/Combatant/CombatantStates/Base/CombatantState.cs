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

        private string inputPrompts = "";
        public string InputPrompts
        {
            get
            {
                return inputPrompts;
            }
            protected set
            {
                inputPrompts = value;
                if (combatant.PrintUItoConsole)
                {
                    Debug.Log($"{combatant.name} UI message: {inputPrompts}", combatant);
                }
            }
        }

        protected CombatantState(
            Combatant combatant,
            Phase phase)
        {
            this.combatant = combatant;
            this.factory = combatant.Factory;
            this.phase = phase;
        }

        /// <summary>
        /// To be called ONCE,
        /// as soon as the state becomes active
        /// </summary>
        public virtual void OnEnter()
        {
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

        // Focus Tile
        protected bool TryGetNewFocus(OverlayTile currentFocus, out OverlayTile newFocus)
        {
            newFocus = combatant.GetNewFocus() ?? combatant.GetDefaultFocus();

            return newFocus != currentFocus;
        }
    }
}
