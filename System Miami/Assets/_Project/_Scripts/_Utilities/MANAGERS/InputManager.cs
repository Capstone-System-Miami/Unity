using UnityEngine;
using System;

namespace SystemMiami
{
    /// <summary>
    /// Manages player input and raises events when specific inputs are detected.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        // Events for different input actions
        public event Action<int> OnAbilityKeyPressed;
        public event Action OnLeftMouseDown;
        public event Action OnRightMouseDown;
        public event Action OnEnterPressed;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            // Check for ability key presses (Alpha1 - Alpha5)
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    // Raise event when ability key is pressed
                    OnAbilityKeyPressed?.Invoke(i);
                }
            }

            // Raise events for mouse clicks and enter key
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftMouseDown?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnRightMouseDown?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnEnterPressed?.Invoke();
            }
        }
    }
}