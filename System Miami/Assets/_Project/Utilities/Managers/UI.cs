// Authors: Layla
using System;
using System.Linq;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

namespace SystemMiami.Management
{
    public class UI : Singleton<UI>
    {
        [Header("Messages")]
        [SerializeField] private bool inputPromptsOn = true;
        [SerializeField] private TextBox inputPromptPanel;

        [Header("GameControl")]
        [SerializeField] private GameObject neighborhoodPauseButtonPrefab;
        [SerializeField] private GameObject dungeonPauseButtonPrefab;
        [SerializeField] private GameObject pausePanelPrefab;
        [SerializeField, ReadOnly] private GameObject pauseButton;
        [SerializeField, ReadOnly] private GameObject pausePanel;

        [Header("CharacterMenu")]
        [SerializeField] private GameObject characterMenuPrefab;
        [SerializeField] private GameObject characterMenuButtonPrefab;
        [SerializeField, ReadOnly] private GameObject characterMenu;
        [SerializeField, ReadOnly] private GameObject characterMenuButton;

        [Header("Loadout UI")]
        [SerializeField] private CombatActionBar physicalAbilitiesBar;
        [SerializeField] private CombatActionBar magicalAbilitiesBar;
        [SerializeField] private CombatActionBar consumablesBar;

        [Header("Combat")]
        [SerializeField] private GameObject lossPanelPrefab;
        [SerializeField] private GameObject winPanelPrefab;
        [SerializeField] private GameObject winBossPanelPrefab;
        [SerializeField] private GameObject rollCreditsPanelPrefab;
        [SerializeField, ReadOnly] private GameObject combatOverPanel;


        public event Action CharacterMenuOpened;
        public event Action CharacterMenuClosed;

        public Action<ActionQuickslot> SlotClicked;
        public Action<Loadout, Combatant> CombatantLoadoutCreated;

        public event EventHandler<DialogueEventArgs> DialogueStarted;
        public event EventHandler DialogueFinished;

        [field: SerializeField, ReadOnly] public GameObject CurrentClient { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;

            GAME.MGR.GamePaused += HandleGamePaused;
            GAME.MGR.GameResumed += HandleGameResumed;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;

            GAME.MGR.GamePaused -= HandleGamePaused;
            GAME.MGR.GameResumed -= HandleGameResumed;
        }

        public void CreatePlayerLoadout(Combatant combatant)
        {
            Assert.IsNotNull(combatant, $"{combatant.name}'s was null");
            OnCreatePlayerLoadout(combatant);
        }

        public void UpdateInputPrompt(string msg)
        {
            if (!inputPromptsOn)
            {
                ClearInputPrompt();
                return ;
            }

            inputPromptPanel.ShowBackground();
            inputPromptPanel.ShowForeground();
            inputPromptPanel.SetForeground(msg);
        }

        public void ClearInputPrompt()
        {
            inputPromptPanel.SetForeground("");
            inputPromptPanel.HideBackground();
            inputPromptPanel.HideForeground();
        }

        public void ClickSlot(ActionQuickslot slot)
        {
            if (TurnManager.MGR == null) { return; }
            if (!TurnManager.MGR.IsPlayerTurn) { return; }
            if (TurnManager.MGR.playerCharacter.CurrentPhase != Phase.Action) { return; }
            physicalAbilitiesBar.DisableAllExcept(slot);
            magicalAbilitiesBar.DisableAllExcept(slot);
            consumablesBar.DisableAllExcept(slot);

            // Raise event
            OnSlotClicked(slot);
        }

        public void StartDialogue(
            object client,
            bool wrapStart,
            bool wrapEnd,
            bool allowCloseEarly,
            string header,
            string[] messages)
        {
            DialogueEventArgs args = new(
                client,
                wrapStart,
                wrapEnd,
                allowCloseEarly,
                header,
                messages
            );

            OnDialogueStarted(args);
        }

        public void StartDialogue(DialogueEventArgs args)
        {
            OnDialogueStarted(args);
        }

        public void FinishDialogue()
        {
            OnDialogueFinished();
        }

        public void OpenCharacterMenu()
        {
            // Create the character menu
            if (characterMenu == null && characterMenuPrefab != null)
            {
                characterMenu = Instantiate(characterMenuPrefab);
            }

            characterMenu.transform.SetParent(transform);

            // Destroy the character menu button
            if (characterMenuButton != null)
            {
                Destroy(characterMenuButton);
                characterMenuButton = null;
            }

            OnCharacterMenuOpened();
        }

        public void CloseCharacterMenu()
        {
            // Create the character menu button
            if (characterMenuButton == null && characterMenuButtonPrefab != null)
            {
                characterMenuButton = Instantiate(characterMenuButtonPrefab);
            }

            characterMenuButton.transform.SetParent(transform);

            // Destroy the character menu
            if (characterMenu != null)
            {
                Destroy(characterMenu);
                characterMenu = null;
            }

            OnCharacterMenuClosed();
        }

        protected virtual void OnCreatePlayerLoadout(Combatant combatant)
        {
            Assert.IsNotNull(combatant, $"{combatant.name} was null");
            Assert.IsNotNull(combatant._inventory, $"{combatant.name}'s Inventory was null");

            Loadout combatantLoadout = new(
                combatant._inventory,
                combatant);

            Assert.IsNotNull(combatantLoadout, $"{combatant.name}'s Loadout was null");

            if (combatant is PlayerCombatant)
            {
                FillLoadoutBars(combatantLoadout);
            }

            CombatantLoadoutCreated?.Invoke(combatantLoadout, combatant);
        }

        protected virtual void OnSlotClicked(ActionQuickslot slot)
        {
            SlotClicked?.Invoke(slot);
        }

        protected virtual void OnDialogueStarted(DialogueEventArgs args)
        {
            CurrentClient = (args.client as MonoBehaviour)?.gameObject;
            DialogueStarted?.Invoke(this, args);
        }

        protected virtual void OnDialogueFinished()
        {
            CurrentClient = null;
            DialogueFinished?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void OnCharacterMenuOpened()
        {
            CharacterMenuOpened?.Invoke();
        }

        protected virtual void OnCharacterMenuClosed()
        {
            CharacterMenuClosed?.Invoke();
        }

        private void FillLoadoutBars(Loadout loadout)
        {
            physicalAbilitiesBar.FillWith(loadout.PhysicalAbilities.Cast<CombatAction>().ToList());
            magicalAbilitiesBar.FillWith(loadout.MagicalAbilities.Cast<CombatAction>().ToList());
            consumablesBar.FillWith(loadout.Consumables.Cast<CombatAction>().ToList());
        }

        private void CreatePauseButton(string sceneName)
        {
            if (pauseButton != null)
            {
                Destroy(pauseButton);
                pauseButton = null;
            }

            GameObject prefab = null;
            if (sceneName == GAME.MGR.NeighborhoodSceneName)
            {
                prefab = neighborhoodPauseButtonPrefab;
            }
            else if (sceneName == GAME.MGR.DungeonSceneName)
            {
                prefab = dungeonPauseButtonPrefab;
            }
            else
            {
                return;
            }

            Assert.IsNotNull(prefab, $"{name} tried to instantiate a null pause button...");

            // Create the button
            pauseButton = Instantiate(prefab);

            pauseButton.transform.SetParent(transform);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == GAME.MGR.DungeonSceneName)
            {
                TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
                TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
            }

            if (scene.name == GAME.MGR.NeighborhoodSceneName)
            {
                OpenCharacterMenu();
                CloseCharacterMenu();
            }

            CreatePauseButton(scene.name);
        }

        private void HandleGamePaused()
        {
            // Create Pause panel
            if (pausePanel == null && pausePanelPrefab != null)
            {
                pausePanel = Instantiate(pausePanelPrefab);
                pausePanel.transform.SetParent(transform);
            }

            // Destroy pause button
            if (pauseButton != null)
            {
                Destroy(pauseButton);
                pauseButton = null;
            }
        }

        private void HandleGameResumed()
        {
            CreatePauseButton(SceneManager.GetActiveScene().name);

            // Destroy pause panel
            if (pausePanel != null)
            {
                Destroy(pausePanel);
                pausePanel = null;
            }
        }

        // TODO: Attach Win/Lose panel script on all the combatOverPanel
        // prefabs. They should take certain information in an Init()
        // and display different things (or connect to different actions)
        // based on the given info.
        //
        private void HandleDungeonCleared()
        {
            Debug.Log($"{name} handling dun clr");

            TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared -= HandleDungeonCleared;

            if (GAME.MGR.AllBossesDefeated)
            {
                combatOverPanel = Instantiate(rollCreditsPanelPrefab);
            }
            else if (GAME.MGR.BossRecentlyDefeated)
            {
                combatOverPanel = Instantiate(winBossPanelPrefab);
            }
            else
            {
                combatOverPanel = Instantiate(winPanelPrefab);
            }

            combatOverPanel.transform.SetParent(transform);
        }

        private void HandleDungeonFailed()
        {
            Debug.Log($"{name} handling dun fail");

            TurnManager.MGR.DungeonFailed -= HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared -= HandleDungeonCleared;

            combatOverPanel = Instantiate(lossPanelPrefab);
        }
    }
}
