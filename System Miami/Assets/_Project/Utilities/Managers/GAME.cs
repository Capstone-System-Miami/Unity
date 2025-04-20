/// Layla
///
#define SYS_PAN_DEVKEYS

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
        #region Debugging
        // =====================================================================
        [Header("Debug Options")]
        public dbug log;

        /// <summary>
        /// WARN: If this is on, DungeonEntrances will
        /// no longer disable after the player leaves them.
        /// </summary>
        [field:
        Tooltip( "Whether or not DungeonEntrance should" + 
                "regenerate data on player exit.\n" + 
                "Enabling this option will make dungeons re-enterable,\n" +
                "so should never be enabled in a release build."),
        SerializeField]
        public bool RegenerateDungeonDataOnInteract { get; private set; } = false;

        /// <summary>
        /// WARN: As of this version, disabling this setting will
        /// have unpredictable results that will likely be
        /// game-breaking.
        /// </summary>
        [field: SerializeField]
        public bool IgnoreObstacles { get; private set; }

        [field:
        Tooltip( "Whether or not Npcs should spawn at all in the game" ),
        SerializeField]
        public bool NoNpcs { get; private set; }

        [SerializeField] DevKeycode debugExitToNeighborhoodImmediate;
        [SerializeField] DevKeycode debugExitToCharSelectImmediate;
        [SerializeField] DevKeycode debugLoseCombatImmediate;
        [SerializeField] DevKeycode debugWinCombatImmediate;
        [SerializeField] DevKeycode debugGoToBossIntersection;
        #endregion // Debugging


        #region Scene Navigation
        // =====================================================================
        [Header("Scene Navigation Settings (CHECK BUILD SETTINGS)"), Space(20)]
        [SerializeField] string mainMenuSceneName;
        [SerializeField] string settingsSceneName;
        [SerializeField] string introSceneName;
        [SerializeField] string characterSelectSceneName;
        [SerializeField] string neighborhoodSceneName;
        [SerializeField] string dungeonSceneName;
        [SerializeField] string outroSceneName;
        [SerializeField] string creditsSceneName;

        public string MainMenuSceneName {
            get { return mainMenuSceneName; }
        }
        public string SettingsSceneName {
            get { return settingsSceneName; }
        }
        public string IntroSceneName {
            get { return introSceneName; }
        }
        public string CharacterSelectSceneName {
            get { return characterSelectSceneName; }
        }
        public string NeighborhoodSceneName {
            get { return neighborhoodSceneName; }
        }
        public string DungeonSceneName {
            get { return dungeonSceneName; }
        }
        public string OutroSceneName {
            get { return outroSceneName; }
        }
        public string CreditsSceneName {
            get { return creditsSceneName; }
        }
        #endregion // Scene Navigation


        [field: Header("Global Settings"), Space(20)]
        [SerializeField] private DungeonPreset[] bosses;

        [field: SerializeField]
        public List<int> LevelThresholds { get; private set; }

        [field: Header("Global Data"), Space(10)]
        [field: SerializeField, ReadOnly]
        public DungeonData CurrentDungeonData { get; private set; }
        [field: SerializeField] public bool BossRecentlyDefeated { get; private set; }

        private Queue<DungeonPreset> bossDungeonQueue = new();

        // Events
        public Action<Combatant> CombatantDying;
        public event Action<Combatant> damageTaken;


        #region Unity Methods / Construction
        // =====================================================================
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

#if SYS_PAN_DEVKEYS
            HookIntoDevKeycodes();
#endif
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
        #endregion // Event Responses


        #region Scene Navigation
        // =====================================================================
        public void GoToMain()
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }

        public void GoToSettings()
        {
            SceneManager.LoadScene(SettingsSceneName);
        }

        public void GoToIntro()
        {
            SceneManager.LoadScene(IntroSceneName);
        }

        public void GoToCharacterSelect()
        {
            log.print($"Going to {CharacterSelectSceneName}");

            Singleton[] managers = FindObjectsOfType<Singleton>(true);
            log.print("Managers Before Pruning", managers);

            for (int i = 0; i < managers.Length; i++)
            {
                if (managers[i] == this) { log.print("me"); continue; }

                Destroy(managers[i].GetMe());
            }
            managers = FindObjectsOfType<Singleton>(true);

            log.print("Managers After Pruning", managers.Select(o => o.name).ToArray());
            SceneManager.LoadScene(CharacterSelectSceneName);
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
            SceneManager.LoadScene(NeighborhoodSceneName);
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

            Debug.Log($"Going to {dungeonSceneName}");
            SceneManager.LoadScene(dungeonSceneName);
        }

        public void GoToOutro()
        {
            SceneManager.LoadScene(OutroSceneName);
        }

        public void GoToCredits()
        {
            SceneManager.LoadScene(CreditsSceneName);
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
        #endregion // Scene Navigation


        #region Global Data
        // =====================================================================
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
            return enemies.Count > 0;
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
        #endregion // Global Data


        #region Debugging
        // =====================================================================
        public int ImmediateBreakpointSpace()
        {
            return 0;
        }

#if SYS_PAN_DEVKEYS
        private void HookIntoDevKeycodes()
        {
            debugExitToNeighborhoodImmediate
                .HookIn(this, () => {
                    GoToNeighborhood(false);
                });
            debugExitToCharSelectImmediate
                .HookIn(this, () => {
                    GoToCharacterSelect();
                });
            debugLoseCombatImmediate
                .HookIn(this, () => {
                    PlayerCombatant player = TurnManager.MGR?.playerCharacter as PlayerCombatant;
                    if (player != null)
                    {
                        player.CurrentState.SwitchState(player.Factory.Dying());
                    }
                });
            debugWinCombatImmediate
                .HookIn(this, () => {
                    List<Combatant> enemies = TurnManager.MGR?.enemyCharacters;
                    if (enemies != null)
                    {
                        foreach (Combatant enemy in enemies)
                        {
                            enemy.CurrentState.SwitchState(enemy.Factory.Dying());
                        }
                    }
                });
            debugGoToBossIntersection
                .HookIn(this, () => {
                    Transform bossIntersectionTransform = IntersectionManager.MGR?.FarthestIntersection.transform;
                    Transform playerTransform = PlayerManager.MGR?.transform;
                    if (bossIntersectionTransform != null && playerTransform != null)
                    {
                        playerTransform.position = bossIntersectionTransform.position;
                    }
                });
        }

        // WARN: This is unused right now...
        // It shouldn't cause any issues, since Game Manager persists
        // through scenes, but it could cause some like "Serialized Reference no longer exists"
        // errors or whatever. Haven't been able to figure out how to unhook without
        // causeing "Routine is null" errors that make the keycodes stop functioning.
        //
        private void UnhookDevKeycodes()
        {
            debugExitToNeighborhoodImmediate.Unhook(this);
            debugExitToCharSelectImmediate.Unhook(this);
            debugLoseCombatImmediate.Unhook(this);
            debugWinCombatImmediate.Unhook(this);
            debugGoToBossIntersection.Unhook(this);
        }
#endif
        #endregion // Debugging


        #region Event Raisers
        // =====================================================================
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
        #endregion // Event Raisers


        #region Event Responses
        // =====================================================================
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
        #endregion // Event Responses
    }
}
