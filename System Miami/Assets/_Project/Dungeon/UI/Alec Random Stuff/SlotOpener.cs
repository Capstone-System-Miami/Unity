using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SystemMiami
{

    public class SlotOpener : MonoBehaviour
    {
        public Button openButton;          // Reference to the Button
        public GameObject[] slots;         // Array to hold slot GameObjects
        private bool areSlotsOpen = false; // Tracks the state of the slots
        private const int maxSlots = 6;    // Maximum number of slots allowed

        void Start()
        {
            // Ensure slots are initially hidden
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null) slots[i].SetActive(false);
            }

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

        void ToggleSlots()
        {
            areSlotsOpen = !areSlotsOpen;

            // Show or hide slots based on the current state and limit
            for (int i = 0; i < Mathf.Min(slots.Length, maxSlots); i++)
            {
                if (slots[i] != null) slots[i].SetActive(areSlotsOpen);
            }
        }
    }


}
