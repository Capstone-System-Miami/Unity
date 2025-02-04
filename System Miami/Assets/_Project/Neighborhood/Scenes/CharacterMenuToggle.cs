using UnityEngine;

public class CharacterMenuToggle : MonoBehaviour
{
    public GameObject characterMenuCanvas;

    void Update()
    {
        /// TODO: This might be impossible to test unless built.
        /// Escape unfocuses the "Game" tab when you're in Play
        /// mode in the Editor so that you can change things
        /// in the inspector.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        characterMenuCanvas.SetActive(!characterMenuCanvas.activeSelf);
    }
}
