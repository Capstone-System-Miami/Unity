using UnityEngine;

namespace SystemMiami.ui
{
    public class GameObjectToggler : MonoBehaviour
    {
        public GameObject gameObjectToToggle; // Reference to the main inventory UI GameObject

        [SerializeField] private KeyCode key;

        [SerializeField] private bool keyTrigger;

        private bool isActive = false; // Tracks whether the object is active

        void Update()
        {
            if (!keyTrigger) { return; }

            // Check if the Escape key is pressed
            if (Input.GetKeyDown(key))
            {
                Toggle();
            }
        }

        // Toggles the UI on/off
        private void Toggle()
        {
            isActive = !isActive; // Toggle the active state
            gameObjectToToggle.SetActive(isActive); // Enable or disable the UI
        }
    }
}