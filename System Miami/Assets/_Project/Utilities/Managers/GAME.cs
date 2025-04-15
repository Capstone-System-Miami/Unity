/// Layla
using System;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Dungeons;
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
        [SerializeField] string _dungeonSceneName;
        [SerializeField] string _neighborhoodSceneName;
        [SerializeField] string characterSelectSceneName;

        [Tooltip(
            "This should really only be checked on while " +
            "debugging / demoing. In game, we want each entrance " +
            "to generate its rewards and enemies when the level loads, " +
            "in case we want to preview rewards, enemies, etc."
            )]
        [SerializeField] private bool _regenerateDungeonDataOnInteract;
        private DungeonData _dungeonData;

        public bool RegenerateDungeonDataOnInteract { get { return _regenerateDungeonDataOnInteract; } }

        [field: SerializeField] public List<int> LevelThresholds { get; private set; }
        [field: SerializeField] public List<DungeonPreset> Bosses { get; private set; }

        [field: SerializeField] public bool IgnoreObstacles { get; private set; }

        [field: SerializeField] public bool NoNpcs { get; private set; }

        public Action<Combatant> CombatantDying;
        public event Action<Combatant> damageTaken;

        protected override void Awake()
        {
            base.Awake();
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
            SceneManager.LoadScene(_dungeonSceneName);
        }

        public void GoToCharacterSelect()
        {
            Debug.Log($"Going to {characterSelectSceneName}");

            Singleton<MonoBehaviour>[] managers = FindObjectsOfType<Singleton<MonoBehaviour>>();
            managers.Where(m => m != null).ToList().ForEach(m => Destroy(m.gameObject));
            // Destroy(PlayerManager.MGR?.gameObject);
            // Destroy(IntersectionManager.MGR?.gameObject);
            SceneManager.LoadScene(characterSelectSceneName);
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

        public void NotifyCombatantDying(Combatant combatant)
        {
            CombatantDying.Invoke(combatant);
        }

        public void NotifyDamageTaken(Combatant combatant)
        {
            damageTaken?.Invoke(combatant);
        }

        public void GoToNeighborhood(bool regenerate)
        {
            _dungeonData = null;

            // local fn for responding to scene load in
            void onSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (regenerate)
                {
                    IntersectionManager.MGR.RegenerateStreets();
                }

                SceneManager.sceneLoaded -= onSceneLoaded;
            }

            // If we're exiting combat and entering into a Neighborhood,
            // it will need to be the one we were in when we entered combat,
            // so we need to turn the IntersectionManager back on
            // before we load the scene. This ensures that the IntersectionManager
            // present in the scene to-be-loaded will destroy itself after it detects
            // the pre-existing IntersectionManager we're carrying with us between scenes.
            if (IntersectionManager.MGR != null && regenerate)
            {
                Destroy(IntersectionManager.MGR);
            }
            else if (IntersectionManager.MGR != null && !regenerate)
            {
                IntersectionManager.MGR.gameObject.SetActive(true);
            }

            Debug.Log($"Going to {_neighborhoodSceneName}");

            SceneManager.sceneLoaded += onSceneLoaded;

            SceneManager.LoadScene(_neighborhoodSceneName);
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
