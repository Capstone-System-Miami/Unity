using UnityEngine;

namespace SystemMiami.CombatSystem
{
    /// <summary>
    /// TODO: Feeling pretty sure at this point that this should
    /// not be its own Subaction?
    /// <para>
    /// Maybe each Subaction should
    /// just have a bool about whether it's a percent-based Subaction.
    /// This class would disolve into 3 Subactions:</para>
    /// <para>
    /// InflictStatusEffect, Damage, and Heal, each with their
    /// boxes checked for <c>percentBased</c>.</para>
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "New Status Effect",
        menuName = "Abilities/CombatActions/Inflict Percent Status Effect")]
    public class InflictPercentStatusEffect : CombatSubactionSO
    {
        [SerializeField] StatSetSO effectStats;
        [SerializeField] float damagePerTurn;
        [SerializeField] float healPerTurn;
        [SerializeField] int durationTurns;
        protected override ISubactionCommand GenerateCommand(ITargetable target)
        {
            return new StatusEffectCommand(
                target,
                new StatSet(effectStats),
                damagePerTurn,
                healPerTurn,
                durationTurns);
        }
    }
}
