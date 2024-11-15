using UnityEngine;
using System;

namespace SystemMiami
{
    /// <summary>
    /// Manages _player input and raises events when specific inputs are detected.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        // Events for different input actions
        public event Action<int> AbilityKeyPressed;
        public event Action LeftMouseDown;
        public event Action RightMouseDown;
        public event Action EnterPressed;

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
                    AbilityKeyPressed?.Invoke(i);
                }
            }

            // Raise events for mouse clicks and enter key
            if (Input.GetMouseButtonDown(0))
            {
                LeftMouseDown?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                RightMouseDown?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                EnterPressed?.Invoke();
            }
        }
    }
}