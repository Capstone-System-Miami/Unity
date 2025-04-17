/// Layla
using System;
using System.Linq;
using SystemMiami.CombatSystem;
using SystemMiami.Dungeons;
using SystemMiami.Utilities;
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
        [Header("Debug")]
        public dbug log;

        [SerializeField] string _dungeonSceneName;
        [SerializeField] string _neighborhoodSceneName;
        [SerializeField] string characterSelectSceneName;
        [SerializeField] string _creditsSceneName;


        [SerializeField] public string DungeonSceneName {
            get { return _dungeonSceneName; }
        }
        [SerializeField] public string NeighborhoodSceneName {
            get { return _neighborhoodSceneName; }
        }
        [SerializeField] public string CharacterSelectSceneName {
            get { return characterSelectSceneName; }
        }
        [SerializeField]
        public string CreditsSceneName
        {
            get { return _creditsSceneName; }
        }

        [Tooltip(
            "This should really only be checked on while " +
            "debugging / demoing. In game, we want each entrance " +
            "to generate its rewards and enemies when the level loads, " +
            "in case we want to preview rewards, enemies, etc."
            )]
        [SerializeField] private bool _regenerateDungeonDataOnInteract;
        public DungeonData CurrentDungeonData { get; private set; }

        /// <summary>
        /// WARN: If this is on, DungeonEntrances will no longer disable after the player leaves them.
        ///
        /// </summary>
        public bool RegenerateDungeonDataOnInteract { get { return _regenerateDungeonDataOnInteract; } }

        [field: SerializeField] public List<int> LevelThresholds { get; private set; }

        [Header("Bosses")]
        [SerializeField] private DungeonPreset[] bosses;
        [field: SerializeField] public bool BossRecentlyDefeated { get; private set; }

        [field: SerializeField] public bool IgnoreObstacles { get; private set; }

        [field: SerializeField] public bool NoNpcs { get; private set; }

        private Queue<DungeonPreset> bossDungeonQueue = new();

        public Action<Combatant> CombatantDying;
        public event Action<Combatant> damageTaken;

        protected override void Awake()
        {
            base.Awake();

            for (int i = 0; i < bosses?.Length; i++)
            {
                bossDungeonQueue.Enqueue(bosses[i]);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Update()
        {
            if (Debug.isDebugBuild
                && Input.GetKeyDown(KeyCode.KeypadMinus)
                && Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                IntersectionManager.MGR.gameObject.SetActive(false);
                GoToCharacterSelect();
            }
        }

        public void NotifyBossSpawned()
        {
            BossRecentlyDefeated = false;
        }

        public void NotifyBossDefeated()
        {
            BossRecentlyDefeated = true;
        }

        public void NotifyCombatantDying(Combatant combatant)
        {
            CombatantDying.Invoke(combatant);
        }

        public void NotifyDamageTaken(Combatant combatant)
        {
            damageTaken?.Invoke(combatant);
        }

        public void GoToCharacterSelect()
        {
            log.on();
            log.print($"Going to {characterSelectSceneName}");

            Singleton[] managers = FindObjectsOfType<Singleton>(true);
            log.print("Managers Before Pruning", managers);

            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i] == this) { log.print("me"); continue; }

                Destroy(managers[i].GetMe());
            }
            managers = FindObjectsOfType<Singleton>(true);

            log.print("Managers After Pruning", managers.Select(o => o.name).ToArray());
            log.stop();
            SceneManager.LoadScene(characterSelectSceneName);
            log.off();
        }

        public void GoToDungeon(DungeonData data)
        {
            CurrentDungeonData = data;

            GoToDungeon();
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

        public void GoToNeighborhood(bool regenerate)
        {
            string debugGotoNeighborhood =
                    $"Going to {NeighborhoodSceneName}.\n" +
                    $"Regenerating?  {regenerate}";

            CurrentDungeonData = null;

            // If we're exiting combat and entering into a Neighborhood,
            // it will need to be the one we were in when we entered combat,
            // so we need to turn the IntersectionManager back on
            // before we load the scene. This ensures that the IntersectionManager
            // present in the scene to-be-loaded will destroy itself after it detects
            // the pre-existing IntersectionManager we're carrying with us between scenes.

            if (IntersectionManager.MGR != null)
            {
                if (regenerate)
                {
                    debugGotoNeighborhood += $"Intersection MGR was {((IntersectionManager.MGR == null) ? "NULL" : "here")}\n";
                    Destroy(IntersectionManager.MGR);
                }
                else // !regen
                {
                    debugGotoNeighborhood += $"Setting active.\n";
                    IntersectionManager.MGR.gameObject.SetActive(true);
                }
            }
            else
            {
                debugGotoNeighborhood += $"Was already null\n";
            }

            debugGotoNeighborhood += $"Going to scene...\n";
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

        public bool TryGetNextBoss(out DungeonPreset preset)
        {
            return bossDungeonQueue.TryDequeue(out preset);
        }

        public bool TryGetEnemies(out List<GameObject> enemies)
        {
            if (CurrentDungeonData == null)
            {
                enemies = new();
                return false;
            }

            enemies = CurrentDungeonData.Enemies;
            return true;
        }

        public bool TryGetDungeonPrefab(out GameObject prefab)
        {
            if (CurrentDungeonData == null)
            {
                prefab = null;
                return false;
            }

            if (CurrentDungeonData.Prefab == null)
            {
                prefab = null;
                return false;
            }

            prefab = CurrentDungeonData.Prefab;
            return true;
        }

        public bool TryGetRewards(out List<ItemData> rewards)
        {
            if (CurrentDungeonData == null)
            {
                rewards = new();
                return false;
            }

            rewards = CurrentDungeonData.ItemRewards;
            return true;
        }

        public bool TryGetEXP(out int exp)
        {
            if (CurrentDungeonData == null)
            {
                exp = 0;
                return false;
            }

            exp = CurrentDungeonData.EXPToGive;
            return true;
        }

        public bool TryGetCredit(out int credit)
        {
            if (CurrentDungeonData == null)
            {
                credit = 0;
                return false;
            }

            credit = CurrentDungeonData.Credits;
            return true;
        }

        public int ImmediateBreakpointSpace()
        {
            return 0;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (TurnManager.MGR != null)
            {
                TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
                TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
            }
        }

        private void HandleDungeonFailed()
        {
            TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared -= HandleDungeonCleared;
        }

        private void HandleDungeonCleared()
        {
            TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared -= HandleDungeonCleared;
            // GoToNeighborhood(CurrentDungeonData.difficulty == DifficultyLevel.BOSS);
        }
    }
}
