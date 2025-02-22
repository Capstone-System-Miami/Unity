using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.Drivers
{
    public class PlayerLevelDriver : MonoBehaviour
    {
        [SerializeField] private TMP_Text currentLevel;
        [SerializeField] private TMP_Text currentXP;
        [SerializeField] private TMP_Text xpToNext;
        [SerializeField] private TMP_InputField xpInput;
        [SerializeField] private TMP_Text buttonText;

        [SerializeField] private PlayerLevel playerLevel;

        private int xpToAdd = int.MinValue;

        private void Update()
        {
            currentLevel.text   = $"Current Level: {playerLevel.CurrentLevel}";
            currentXP.text      = $"Current XP: {playerLevel.CurrentXP}";
            xpToNext.text       = $"XP to Next Level: {playerLevel.XPtoNextRemaining}";

            buttonText.text = (xpToAdd != int.MinValue)
                                ? $"Gain {xpToAdd} XP"
                                : $"Set an integer XP amount in the box above";
        }

        public void EditEnded()
        {
            ParseForInt(xpInput.text);
        }

        public void ParseForInt(string s)
        {
            xpToAdd = int.TryParse(s, out int result)
                ? result
                : int.MinValue;
        }

        public void AddXP()
        {
            if (xpToAdd == int.MinValue) { return; }

            playerLevel.GainXP(xpToAdd);
        }        
    }
}
