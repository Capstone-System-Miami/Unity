using System;
using SystemMiami.CombatSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ENGINE
using UnityEditor;
#endif

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

        public void Quit()
        {
            #if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
            #else
            Application.Quit();
            #endif
        }
    }
}
