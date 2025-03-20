using SystemMiami.InventorySystem;
using UnityEngine;

namespace SystemMiami
{
    public class CharacterResourcesPanel : MonoBehaviour
    {
        [field: SerializeField, ReadOnly] private PlayerLevel playerLevel { get; set; }
        [field: SerializeField, ReadOnly] private Attributes attributes { get; set; }

        [Header("Internal Refs")]
        [SerializeField] private LabeledField levelField;
        [SerializeField] private LabeledField xpField;
        [SerializeField] private LabeledField creditsField;
        [SerializeField] private LabeledField pointsField;

        private int level;
        private float xpToNextCurrent;
        private float xpToNextTotal;
        private int credits;
        private int attributePoints;


        private void Awake()
        {
            playerLevel = PlayerManager.MGR.gameObject.GetComponent<PlayerLevel>();
            attributes = PlayerManager.MGR.gameObject.GetComponent<Attributes>();
        }

        private void Update()
        {
            if (playerLevel == null) { return; }
            if (attributes == null) { return; }

            UpdateVals();
            UpdateUIComponents();
        }

        public void UpdateVals()
        {
            level = playerLevel.CurrentLevel;
            xpToNextCurrent = playerLevel.CurrentXP;
            xpToNextTotal = playerLevel.XpToNextTotal;
            credits = PlayerManager.MGR.CurrentCredits;
            attributePoints = attributes.TotalPointsAvailable;
        }

        private void UpdateUIComponents()
        {
            levelField.Value.SetForeground($"{level}");
            xpField.Value.SetForeground($"{xpToNextCurrent}/{xpToNextTotal}");
            creditsField.Value.SetForeground($"${credits}");
            pointsField.Value.SetForeground($"{attributePoints}");
        }
    }
}
