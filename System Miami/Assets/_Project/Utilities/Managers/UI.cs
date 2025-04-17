// Authors: Layla
using System;
using System.Linq;
using SystemMiami.CombatRefactor;
using SystemMiami.CombatSystem;
using System.Collections;
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

        [Header("Loadout UI")]
        [SerializeField] private CombatActionBar physicalAbilitiesBar;
        [SerializeField] private CombatActionBar magicalAbilitiesBar;
        [SerializeField] private CombatActionBar consumablesBar;

        [Header("Game Over")]
        [SerializeField] private GameObject lossPanelPrefab;
        [SerializeField] private GameObject winPanelPrefab;
        // This should eventually get removed when we have a substantial win panel.
        [SerializeField] private GoToNeighborhood TEMP_goToNeighborhood;

        public Action<ActionQuickslot> SlotClicked;
        public Action<Loadout, Combatant> CombatantLoadoutCreated;

        public event EventHandler<DialogueEventArgs> DialogueStarted;
        public event EventHandler DialogueFinished;

        [field: SerializeField, ReadOnly] public GameObject CurrentClient { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
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

        protected virtual void OnCreatePlayerLoadout(Combatant combatant)
        {
            Loadout combatantLoadout = new(
                combatant._inventory,
                combatant);
            Assert.IsNotNull(combatant._inventory, $"{combatant.name}'s Inventory was null");
            Assert.IsNotNull(combatant, $"{combatant.name} was null");
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
            DialogueFinished?.Invoke(this, EventArgs.Empty);
        }

        private void FillLoadoutBars(Loadout loadout)
        {
            physicalAbilitiesBar.FillWith(loadout.PhysicalAbilities.Cast<CombatAction>().ToList());
            magicalAbilitiesBar.FillWith(loadout.MagicalAbilities.Cast<CombatAction>().ToList());
            consumablesBar.FillWith(loadout.Consumables.Cast<CombatAction>().ToList());
        }


        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (TurnManager.MGR != null)
            {
                TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
                TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
            }
        }

        private void HandleDungeonCleared()
        {
            Debug.Log($"{name} handling dun clr");
            // TEMP_goToNeighborhood.Go(GAME.MGR.CurrentDungeonData.difficulty == Dungeons.DifficultyLevel.BOSS);

            // TODO:
            // Comment the above line and uncomment this when the
            // panel is functional/ ready to be used.
            //
            Instantiate(winPanelPrefab);

            TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
        }

        private void HandleDungeonFailed()
        {
            Debug.Log($"{name} handling dun fail");
            Instantiate(lossPanelPrefab);

            TurnManager.MGR.DungeonFailed += HandleDungeonFailed;
            TurnManager.MGR.DungeonCleared += HandleDungeonCleared;
        }
    }
}
