using UnityEngine;

public class CharacterMenuToggle : MonoBehaviour
{
    public GameObject characterMenuCanvas;

    void Update()
    {
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
