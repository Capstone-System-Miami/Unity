using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SystemMiami.ui
{
    public class EndCombatWindow : MonoBehaviour
    {
        [Header("Sprite Replacements")]
        [SerializeField] private Image charImage;
        [SerializeField, ReadOnly] private Sprite charSprite;

        [Header("Specific String Replacements")]
        [SerializeField] private string bossToken;
        [SerializeField] private string bossPowerToken;

        [Header("General String Replacements (expensive)")]
        [SerializeField] private List<StringReplacementHelper> replacements;
        [SerializeField, ReadOnly] TMP_Text[] allTexts;

        private StringReplacementHelper bossReplace;
        private StringReplacementHelper bossPowerReplace;

        protected virtual void Awake()
        {
            // expensive
            allTexts = GetComponentsInChildren<TMP_Text>();
        }

        private void OnDisable()
        {
            // Clear char sprite
            if (charImage != null)
            {
                charImage.sprite = null;
            }
            ClearBossTokens();
        }

        public void Init(BossTag bossTag)
        {
            // Set char sprite
            if (charImage != null)
            {
                charSprite = PlayerManager.MGR.PlayerSprite;
                charImage.sprite = charSprite;
            }

            if (bossTag != null)
            {
                StoreBossTokens(bossTag);
            }

            // spensive
            ReplaceTokens();
        }

        public void ReplaceTokens()
        {
            for (int i = 0; i < allTexts.Length; i++)
            {
                for (int j = 0; j < replacements.Count; j++)
                {
                    allTexts[i].text = allTexts[i].text
                        .Replace(replacements[j].token, replacements[j].replacement);
                }
            }
        }

        public void StoreBossTokens(BossTag boss)
        {
            bossReplace = new (bossToken, boss.bossName);
            bossPowerReplace = new (bossPowerToken, boss.powerName);
            replacements.Add(bossReplace);
            replacements.Add(bossPowerReplace);
        }
        public void ClearBossTokens()
        {
            bossReplace = new();
            bossPowerReplace = new();
        }
    }

    [System.Serializable]
    public class StringReplacementHelper
    {
        [field: SerializeField] public string token { get; private set; }
        [field: SerializeField] public string replacement { get; private set; }

        public StringReplacementHelper() : this ("", "") { }

        public StringReplacementHelper(string token, string replacement)
        {
            this.token = token;
            this.replacement = replacement;
        }
    }
}
