using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemMiami
{
    public class ButtonSceneLoader : MonoBehaviour
    {
        // Function to load a scene by its name
        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("Scene name is empty or null! Please provide a valid scene name.");
            }
        }

        // Function to quit the game
        public void QuitGame()
        {
            Debug.Log("Game is quitting..."); // Works in the editor for feedback
            Application.Quit(); // Exits the application in a built game
        }
    }

}
