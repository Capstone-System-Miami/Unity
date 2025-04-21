using SystemMiami.Management;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class SlotOpener : MonoBehaviour
    {
        public Button openButton;          // Reference to the Button
        public GameObject[] slots;         // Array to hold slot GameObjects
        private bool areSlotsOpen = false; // Tracks the state of the slots
        private bool stateOnDisable;      // State of this obj when GAME.MGR.GamePaused last called
        private const int maxSlots = 6;    // Maximum number of slots allowed

        private void OnEnable()
        {
            GAME.MGR.GamePaused += HandlePauseGame;
            GAME.MGR.GameResumed += HandleResumeGame;
        }

        private void OnDisable()
        {
            GAME.MGR.GamePaused -= HandlePauseGame;
            GAME.MGR.GameResumed -= HandleResumeGame;
        }

        void Start()
        {
            // Ensure slots are initially hidden
            EnableInteraction(false);
            CloseAll();

            // Add a listener to the button
            if (openButton != null)
            {
                openButton.onClick.AddListener(ToggleSlots);
            }
            else
            {
                Debug.LogError("Open Button is not assigned!");
            }
        }

        public void EnableInteraction(bool andRestoreState)
        {
            openButton.interactable = true;

            if (andRestoreState)
            {
                SetAll(stateOnDisable);
            }
        }

        public void DisableInteraction()
        {
            openButton.interactable = false;

            stateOnDisable = areSlotsOpen;
        }

        public void ToggleSlots()
        {
            Action toggle = areSlotsOpen ? CloseAll : OpenAll;
            toggle();
        }

        public void OpenAll()
        {
            SetAll(true);
        }

        public void CloseAll()
        {
            SetAll(false);
        }

        private void SetAll(bool value)
        {
            areSlotsOpen = value;
            for (int i = 0; i < Mathf.Min(slots.Length, maxSlots); i++)
            {
                slots[i]?.SetActive(value);
            }
        }


        // Event Responses
        private void HandlePauseGame()
        {
            DisableInteraction();
            CloseAll();
        }
        private void HandleResumeGame()
        {
            EnableInteraction(true);
        }
    }
}
