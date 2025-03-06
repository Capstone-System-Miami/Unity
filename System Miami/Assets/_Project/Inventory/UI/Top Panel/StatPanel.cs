using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class StatPanel : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [SerializeField] private Stats playerStats;

        [SerializeField] private StatSetVisualizer currentStatsVisualizer;

        private StatSet Current => playerStats.BeforeEffectsCopy;

        private void Start()
        {
            playerStats = PlayerManager.MGR.GetComponent<Stats>();
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
