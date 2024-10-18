using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class StatusEffect 
    {
        public AttributeSet Effect { get; private set; }
        public int Duration { get; private set; }
        
        // Start is called before the first frame update
        public StatusEffect(AttributeSet effect, int duration)
        {
            Effect = effect;
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
