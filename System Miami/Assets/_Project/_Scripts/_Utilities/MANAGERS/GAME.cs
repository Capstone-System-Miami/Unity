using System;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemMiami.Management
{
    public class GAME : Singleton<GAME>
    {
        public Action<Combatant> CombatantDeath;

        [SerializeField] int _dungeonIndex;
        [SerializeField] int _neighborhoodIndex;

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
            switchScene(_dungeonIndex);
        }

        public void GoToNeighborhood()
        {
            switchScene(_neighborhoodIndex);
        }

    }
}
