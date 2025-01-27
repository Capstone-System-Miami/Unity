using System.Collections;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public class CombatantStateMachine : MonoBehaviour
    {
        //private Combatant combatant;

        //public Phase CurrentPhase
        //{
        //    get
        //    {
        //        return currentState.phase;
        //    }
        //}

        //public bool ReadyToStart
        //{
        //    get
        //    {
        //        return currentState is Idle;
        //    }
        //}

        //private CombatantState currentState;

        //// ========================================

        //private void Awake()
        //{
        //    if (!TryGetComponent(out combatant))
        //    {
        //        Debug.LogError(
        //            $"Didnt find a Combatant" +
        //            $"component on {name}.");
        //    }
        //}

        //private void Start()
        //{
        //    CombatantState startState = combatant.IsPlayer ?
        //        new PlayerIdle(combatant)
        //        : new EnemyIdle(combatant);

        //    SetState(startState);
        //}

        //private void Update()
        //{
        //    if (currentState == null)
        //    {
        //        Debug.LogError(
        //            $"{gameObject}'s {name}'s" +
        //            $"currentState is null."
        //            );
        //    }

        //    // state manage
        //    currentState.MakeDecision();
        //    currentState.Update();
        //}

        //public void LateUpdate()
        //{
        //    currentState.LateUpdate();
        //}

        ///// <summary>
        ///// The main method by which state transitions happen.
        ///// </summary>
        //public void SetState(CombatantState newState)
        //{
        //    if (newState == null)
        //    {
        //        Debug.Log(
        //            $"{name}'s {this}'s SetState() was" +
        //            $"passed a null state."
        //            );
        //        return;
        //    }

        //    /// Call OnExit on
        //    /// the current state object
        //    /// before setting a new one
        //    /// (The '?' means only call this
        //    /// if the object is not "null".
        //    currentState?.OnExit();

        //    /// Set the current state to the new state.
        //    /// passed into this function as an arg.
        //    currentState = newState;

        //    /// Call OnEnter on the new state object
        //    currentState.OnEnter();
        //}

        ///// <summary>
        ///// An overload of SwitchState that takes a float.
        ///// This function will run a Coroutine that
        /////  then
        ///// waits `float delay` seconds before assigning
        ///// a new state and calling OnEnter() on it.
        ///// </summary>
        ///// 
        ///// <param name="newState">
        ///// The state to transition to.
        ///// </param>
        ///// 
        ///// <param name="delay">
        ///// Amount of time in seconds for
        ///// the state machine to wait between: 
        ///// (a) calling OnExit() on the previous state, and
        ///// (b) calling OnEnter() on the new current state.
        ///// </param>
        //public void SwitchState(CombatantState newState, float delay)
        //{
        //    StartCoroutine(switchStateWithDelay(newState, delay));
        //}

        ///// <summary>
        ///// The IEnumerator by which the
        ///// state machine can perform state changes
        ///// with a delay.
        ///// </summary>
        ///// 
        ///// <returns>
        ///// The enumerated process
        ///// </returns>
        //private IEnumerator switchStateWithDelay(CombatantState newState, float delay)
        //{
        //    currentState.OnExit();

        //    yield return new WaitForSeconds(delay);

        //    currentState = newState;
        //    currentState.OnEnter();
        //}
    }
}
