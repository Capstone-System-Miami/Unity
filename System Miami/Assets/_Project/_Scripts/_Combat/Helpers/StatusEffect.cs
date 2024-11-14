using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class StatusEffect 
    {
        public StatSet Effect { get; private set; }
        public int Duration { get; private set; }

        public StatusEffect(StatSetSO effect, int duration)
        {
            Effect = new StatSet(effect);
            Duration = duration;
        }

        /// <summary>
        /// Decreases the duration of the status effect by one turn.
        /// </summary>
        public void DecrementDuration()
        {
            Duration--;
        }

        /// <summary>
        /// Checks if the status effect has expired.
        /// </summary>
        public bool IsExpired()
        {
            return Duration <= 0;
        }
    }
}
