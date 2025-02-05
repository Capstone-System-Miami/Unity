using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class StatusEffect
    {
        public StatSet Effect { get; private set; }
        public float DamagePerTurn { get; private set; }
        public int DurationTurns { get; private set; }


        public StatusEffect(StatSetSO effect, int duration)
        {
            Effect = new StatSet(effect);
            DamagePerTurn = 0f;
            DurationTurns = duration;
        }
        public StatusEffect(StatSetSO effect, float damage, int duration)
        {
            Effect = new StatSet(effect);
            DamagePerTurn = damage;
            DurationTurns = duration;
        }
        /// <summary>
        /// Decreases the duration of the status effect by one turn.
        /// </summary>
        public void DecrementDuration()
        {
            DurationTurns--;
        }
        /// <summary>
        /// Checks if the status effect has expired.
        /// </summary>
        public bool IsExpired()
        {
            return DurationTurns <= 0;
        }
    }
}
