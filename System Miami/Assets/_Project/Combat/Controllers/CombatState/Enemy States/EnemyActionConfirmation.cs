using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{

    public class EnemyActionConfirmation : ActionConfirmation
    {
        private const float DELAY = 0.5f;
        private CountdownTimer delayBetweenChecks;

        private Conditions cancelRequestConditions = new();
        private Conditions endTurnRequestConditions = new();

        /// <summary>
        /// A STATIC variable, so we can use this as we come back
        /// to the state between checks. Should be reset if
        /// the enemy is transitioning forward to:
        /// <para>
        /// a) EnemyActionExecution</para>
        /// <para>
        /// b) EnemyTurnEnd</para>
        /// </summary>
        private static int tilesToCheck = 4;

        public EnemyActionConfirmation(Combatant combatant, CombatAction combatAction)
            : base(combatant, combatAction) { }

        public override void OnEnter()
        {
            base.OnEnter();
            delayBetweenChecks = new(combatant, DELAY);
            delayBetweenChecks.Start();

            cancelRequestConditions.Add(
                () => delayBetweenChecks.IsStarted);
            cancelRequestConditions.Add(
                () => delayBetweenChecks.IsFinished);
            cancelRequestConditions.Add(
                () => !combatAction.PlayerFoundInTargets());
            cancelRequestConditions.Add(
                () => tilesToCheck > 0);

            endTurnRequestConditions.Add(
                () => delayBetweenChecks.IsStarted);
            endTurnRequestConditions.Add(
                () => delayBetweenChecks.IsFinished);
            endTurnRequestConditions.Add(
                () => !combatAction.PlayerFoundInTargets());
            endTurnRequestConditions.Add(
                () => tilesToCheck <= 0);
        }

        public override void MakeDecision()
        {
            base.MakeDecision();

            if (EndTurn())
            {
                Debug.LogWarning($"Requesting turn end", combatant);
                SwitchState(factory.TurnEnd());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (CancelConfirmation())
            {
                Debug.LogWarning(
                    $"Requested cancel confirmation duing confirmation",
                    combatant);

                tilesToCheck--;
                return;
            }

            if (EndTurn())
            {
                Debug.LogWarning(
                    $"Requested end turn during confirmation",
                    combatant);

                tilesToCheck = 4;
                return;
            }
        }

        protected override bool CancelConfirmation()
        {
            return cancelRequestConditions.AllMet();
        }

        protected override bool ConfirmSelection()
        {
            return combatAction.PlayerFoundInTargets();
        }

        protected bool EndTurn()
        {
            return endTurnRequestConditions.AllMet();
        }
    }
}
