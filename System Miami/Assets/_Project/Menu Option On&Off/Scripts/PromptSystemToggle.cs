using UnityEngine;
using UnityEngine.UI;

public class PromptSystemToggle : MonoBehaviour
{
    public Toggle promptToggle;
    public GameObject promptText;

    void Start()
    {
        // Load the last saved preference (1 = on, 0 = off)
        bool isEnabled = PlayerPrefs.GetInt("PromptEnabled", 1) == 1;
        promptText.SetActive(isEnabled);
        promptToggle.isOn = isEnabled;

        // Add listener for toggle changes
        promptToggle.onValueChanged.AddListener(delegate { TogglePrompt(promptToggle.isOn); });
    }

    void TogglePrompt(bool isOn)
    {
        promptText.SetActive(isOn);
        PlayerPrefs.SetInt("PromptEnabled", isOn ? 1 : 0);
    }
}
