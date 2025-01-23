using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class CombatantStateMachine : MonoBehaviour
    {
        public float movementSpeed;

        [HideInInspector] public Combatant combatant;



        // Abilities
        public AbilityType TypeToEquip;
        public int IndexToEquip;

        public Phase CurrentPhase
        {
            get
            {
                return currentState.Phase;
            }
        }

        protected CombatantState currentState;


        // ========================================

        private void Awake()
        {
            if (!TryGetComponent(out combatant))
            {
                Debug.LogError($"Didnt find a Combatant component on {name}.");
            }
        }

        private void Start()
        {
            bool isPlayer =
                (combatant.gameObject == PlayerManager.MGR.gameObject);

            CombatantState startState = isPlayer ?
                new PlayerTurnStart(this)
                : new EnemyTurnStart(this);

            SwitchState(startState);
        }

        private void Update()
        {
            // state manage
            currentState.bUpdate();
            currentState.cMakeDecision();
        }

        public void LateUpdate()
        {
            currentState.dLateUpdate();
        }

        /// <summary>
        /// The main method by which state transitions happen.
        /// </summary>
        public void SwitchState(CombatantState newState)
        {
            /// Call OnExit on
            /// the current state object
            /// before setting a new one
            /// (The '?' means only call this
            /// if the object is not "null".
            currentState?.eOnExit();

            /// Set the current state to the new state.
            /// passed into this function as an arg.
            currentState = newState;

            /// Call OnEnter on the new state object
            currentState.aOnEnter();
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
            StartCoroutine(switchStateWithDelay(newState, delay));
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
        private IEnumerator switchStateWithDelay(CombatantState newState, float delay)
        {
            currentState.eOnExit();

            yield return new WaitForSeconds(delay);

            currentState = newState;
            currentState.aOnEnter();
        }
    }
}
