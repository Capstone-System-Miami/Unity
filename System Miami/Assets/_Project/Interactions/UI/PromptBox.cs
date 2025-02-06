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
            string prompt = $"Press {interactKey} ";
            string actionPrompt = interaction.GetActionPrompt();

            if (interaction.GetActionPrompt() != null
                && interaction.GetActionPrompt() != "")
            {
                prompt += $"to {actionPrompt}";
            }
            ///TODO: This is truly duct tape and spit
            else if (interaction is MonoBehaviour m)
            {
                if (m.transform.parent.TryGetComponent(out DungeonEntrance entrance))
                {
                    prompt += "to Enter";
                }
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
