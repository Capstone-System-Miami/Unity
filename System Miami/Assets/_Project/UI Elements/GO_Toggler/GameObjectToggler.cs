using UnityEngine;
using System.Collections.Generic;

namespace SystemMiami.ui
{
    public class GameObjectToggler : MonoBehaviour
    {
        /// <summary>
        /// A list of objects that will be toggled via SetActive()
        /// </summary>
        public List<GameObject> gameObjectsToToggle;

        /// <summary>
        /// Whether the object should be set active on this.Start()
        /// </summary>
        public bool startActive = false;

        /// <summary>
        /// The key that will toggle the object if it is set to trigger on
        /// a key press
        /// See <see cref="keyTrigger"/>
        /// </summary>
        [SerializeField] private KeyCode key;

        /// <summary>
        /// Whether the object should be toggled by a key press
        /// </summary>
        [SerializeField] private bool keyTrigger;

        // Tracks whether the object is active
        private bool isActive = false;

        private void Start()
        {
            // Set each active state to the startActive value
            gameObjectsToToggle.ForEach(go => go.SetActive(startActive));
        }

        private void Update()
        {
            // If this isn't using a key trigger, return early
            if (!keyTrigger) { return; }

            // Check if the Escape key is pressed
            if (Input.GetKeyDown(key))
            {
                Toggle();
            }
        }

        // Toggles the UI on/off
        public void Toggle()
        {
            // Toggle the active state
            isActive = !isActive;

            // Enable or disable the gameObject
            gameObjectsToToggle.ForEach(go => go.SetActive(isActive));
        }
    }
}