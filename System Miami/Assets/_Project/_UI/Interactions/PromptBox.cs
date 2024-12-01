using UnityEngine;

namespace SystemMiami
{
    public class PromptBox : TextBox
    {
        private void Start()
        {
            Clear();
        }

        public void ShowPrompt(IInteractable interaction, KeyCode interactKey)
        {
            string prompt = $"Press {interactKey}";
            string actionPrompt = interaction.GetActionPrompt();

            if (interaction.GetActionPrompt() != null)
            {
                prompt += $" to {actionPrompt}";
            }

            ShowBackground();
            ShowForeground();

            SetForeground(prompt);
        }

        public void Clear()
        {
            SetForeground("");

            HideBackground();
            HideForeground();
        }
    }
}
