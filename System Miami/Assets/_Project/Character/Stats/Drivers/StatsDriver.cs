using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class StatsDriver : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [SerializeField] private Stats playerStats;

        [SerializeField] private StatSetVisualizer currentStatsVisualizer;

        private StatSet Current => playerStats.BeforeEffectsCopy;

        private void Awake()
        {
            if (playerStats == null)
            {
                log.error(
                    $"{name}'s playerStats variable was not" +
                    $"assigned in the inspector.");
                return;
            }
        }

        private void Update()
        {
            RefreshValues();
        }

        public void RefreshValues()
        {
            currentStatsVisualizer.Assign(Current);
        }
    }

}
