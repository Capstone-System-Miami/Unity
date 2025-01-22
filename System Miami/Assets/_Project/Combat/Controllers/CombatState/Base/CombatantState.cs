namespace SystemMiami.CombatRefactor
{
    public abstract class CombatantState
    {
        public readonly Phase Phase;

        protected readonly CombatantStateMachine machine;



        protected CombatantState(
            CombatantStateMachine machine,
            Phase phase
            )
        {
            this.machine = machine;
            Phase = phase;
        }

        /// <summary>
        /// To be called ONCE,
        /// as soon as the state becomes active
        /// </summary>
        public abstract void aOnEnter();

        /// <summary>
        /// To be called EVERY FRAME while the state is active
        /// Anything that needs to be checked or adjusted
        /// That doesn't have to do with transitioning
        /// to the next state should be implemented here.
        /// </summary>
        public abstract void bUpdate();

        /// <summary>
        /// To be called EVERY FRAME after Update().
        /// This is where decisions should be made about
        /// whether to remain in this state,
        /// or transition into a new state
        /// (and if so, which new state? etc.)
        /// </summary>
        public abstract void cMakeDecision();

        public virtual void dLateUpdate() { }

        /// <summary>
        /// Called once, inside TransitionTo(newState),
        /// just before the state becomes inactive
        /// </summary>
        public abstract void eOnExit();
    }
}