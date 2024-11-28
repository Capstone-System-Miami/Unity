using System;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemMiami.Management
{
    public class GAME : Singleton<GAME>
    {
        public Action<Combatant> CombatantDeath;

        [SerializeField] string _dungeonSceneName;
        [SerializeField] string _neighborhoodSceneName;

        protected override void Awake()
        {
            base.Awake();
        }
        private void switchScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex);
        }

        private void switchScene(string buildName)
        {
            SceneManager.LoadScene(buildName);
        }

        public void GoToDungeon()
        {
            switchScene(_dungeonSceneName);
        }

        public void GoToNeighborhood()
        {
            switchScene(_neighborhoodSceneName);
        }
    }
}
