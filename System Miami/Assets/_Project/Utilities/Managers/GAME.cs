/// Layla
using System;
using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

        [Tooltip(
            "This should really only be checked on while " +
            "debugging / demoing. In game, we want each entrance " +
            "to generate its rewards and enemies when the level loads, " +
            "in case we want to preview rewards, enemies, etc."
            )]
        [SerializeField] private bool _regenerateDungeonDataOnInteract;
        private DungeonData _dungeonData;

        public bool RegenerateDungeonDataOnInteract { get { return _regenerateDungeonDataOnInteract; } }

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

        /// <summary>
        /// Doesn't take DungeonEntrances/Difficulty/DungeonPresets into account.
        /// </summary>
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

            Debug.Log($"Going to {_dungeonSceneName}");
            switchScene(_dungeonSceneName);
        }

        public void GoToDungeon(DungeonData data)
        {
            _dungeonData = data;

            GoToDungeon();
        }

        public bool TryGetEnemies(out List<GameObject> enemies)
        {
            if (_dungeonData == null)
            {
                enemies = new();
                return false;
            }

            enemies = _dungeonData.Enemies;
            return true;
        }

        public bool TryGetDungeonPrefab(out GameObject prefab)
        {
            if (_dungeonData == null)
            {
                prefab = null;
                return false;
            }

            if (_dungeonData.Prefab == null)
            {
                prefab = null;
                return false;
            }

            prefab = _dungeonData.Prefab;
            return true;
        }

        public bool TryGetRewards(out List<ItemData> rewards)
        {
            if (_dungeonData == null)
            {
                rewards = new();
                return false;
            }
            

            rewards = _dungeonData.ItemRewards;
            return true;
        }

        public bool TryGetEXP(out int exp)
        {
            if (_dungeonData == null)
            {
                exp = 0;
                return false;
            }

            exp = _dungeonData.EXPToGive;
            return true;
        }

        public bool TryGetCredit(out int credit)
        {
            if (_dungeonData == null)
            {
                credit = 0;
                return false;
            }

            credit = _dungeonData.Credits;
            return true;
        }

        public void GoToNeighborhood()
        {
            _dungeonData = null;

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

            Debug.Log($"Going to {_neighborhoodSceneName}");
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

        public int ImmediateBreakpointSpace()
        {
            return 0;
        }
    }
}
