using System;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
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
            // If we're in a Neighborhood and are entering combat,
            // we should turn off the IntersectionManager,
            // since it will be preserved between scenes. 
            if (IntersectionManager.MGR != null)
            {
                IntersectionManager.MGR.gameObject.SetActive(false);

                // Store the player's position to return to when
                // re-entering the Neighborhood.
                PlayerManager.MGR.StoreNeighborhoodPosition();
            }

            switchScene(_dungeonSceneName);
        }

        public void GoToNeighborhood()
        {
            // If we're exiting combat and entering into a Neighborhood,
            // it will need to be the one we were in when we entered combat,
            // so we need to turn the IntersectionManager back on
            // before we load the scene. This ensures that the IntersectionManager
            // present in the scene to-be-loaded will destroy itself after it detects
            // the pre-existing IntersectionManager we're carrying with us between scenes.
            if (IntersectionManager.MGR != null)
            {
                IntersectionManager.MGR.gameObject.SetActive(true);            
            }

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
