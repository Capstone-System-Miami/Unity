using System;
using System.Collections.Generic;
using SystemMiami.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class AttributesDriver : MonoBehaviour
    {
        [SerializeField] dbug log;

        [Header("Component")]
        [SerializeField] private Attributes playerAttributes;

        [Header("Info Text")]
        [SerializeField] private GameObject upgradeMessagesPanel;
        [SerializeField] private TMP_Text upgradeMessages;
        [SerializeField] private LabeledField attributePointsField;

        [Header("Attribute Set Visualizers")]
        [SerializeField] private AttributeSetVisualizer currentVisualizer;
        [SerializeField] private AttributeSetVisualizer upgradesVisualizer;
        [SerializeField] private AttributeSetVisualizer previewVisualizer;

        [Header("Add & Subtract buttons")]
        [Space(5)]
        [SerializeField] private Button addStr;
        [SerializeField] private Button subtractStr;
        [Space(5)]
        [SerializeField] private Button addDex;
        [SerializeField] private Button subtractDex;
        [Space(5)]
        [SerializeField] private Button addCon;
        [SerializeField] private Button subtractCon;
        [Space(5)]
        [SerializeField] private Button addWis;
        [SerializeField] private Button subtractWis;
        [Space(5)]
        [SerializeField] private Button addInt;
        [SerializeField] private Button subtractInt;

        private AttributeSet Current => playerAttributes.CurrentCopy;
        private AttributeSet Upgrade => playerAttributes.UpgradesCopy;
        private AttributeSet Preview => playerAttributes.PreviewCopy;

        private Dictionary<AttributeType, Button> subtractPointButtons = new();
        private Dictionary<AttributeType, Button> addPointButtons = new();

        private CountdownTimer messageTimer;

        private void Awake()
        {
            if (playerAttributes == null)
            {
                log.error(
                    $"No Attributes component was assigned to {name}" +
                    $"in the inspector.");
            }

            messageTimer = new(this, 2f);
        }

        private void Start()
        {
            playerAttributes.SetClass(CharacterClassType.FIGHTER);

            RefreshValues();

            FillDictionaries();

            CancelUpgrade();

            ClearUpgradeMessage();
        }

        private void Update()
        {
            attributePointsField.Value.SetForeground(
                $"{playerAttributes.PointsRemaining}");

            if (messageTimer!= null && messageTimer.IsFinished)
            {
                ClearUpgradeMessage();
            }

            RefreshValues();
        }

        public void EnterUpgradeMode()
        {
            currentVisualizer.gameObject.SetActive(true);
            upgradesVisualizer.gameObject.SetActive(true);
            previewVisualizer.gameObject.SetActive(true);

            playerAttributes.EnterUpgradeMode();
        }

        public void CancelUpgrade()
        {
            currentVisualizer.gameObject.SetActive(true);
            upgradesVisualizer.gameObject.SetActive(false);
            previewVisualizer.gameObject.SetActive(false);

            playerAttributes.LeaveUpgradeMode();
        }

        public void ConfirmUpgrade()
        {
            currentVisualizer.gameObject.SetActive(true);
            upgradesVisualizer.gameObject.SetActive(false);
            previewVisualizer.gameObject.SetActive(false);

            playerAttributes.ConfirmUpgrades();
        }

        public void RefreshValues()
        {
            currentVisualizer.Assign(Current);
            upgradesVisualizer.Assign(Upgrade);
            previewVisualizer.Assign(Preview);
        }

        private void FillDictionaries()
        {
            // Define dictionaries
            subtractPointButtons = new Dictionary<AttributeType, Button>
            {
                { AttributeType.STRENGTH, subtractStr },
                { AttributeType.DEXTERITY, subtractDex },
                { AttributeType.CONSTITUTION, subtractCon },
                { AttributeType.WISDOM, subtractWis },
                { AttributeType.INTELLIGENCE, subtractInt },
            };

            addPointButtons = new Dictionary<AttributeType, Button>
            {
                { AttributeType.STRENGTH, addStr },
                { AttributeType.DEXTERITY, addDex },
                { AttributeType.CONSTITUTION, addCon },
                { AttributeType.WISDOM, addWis },
                { AttributeType.INTELLIGENCE, addInt },
            };

            // Add listeners to buttons
            for (int i = 0; i < CharacterEnums.ATTRIBUTE_COUNT; i++)
            {
                AttributeType type = (AttributeType)i;

                subtractPointButtons[type].onClick.AddListener(
                    () => SubtractFromUpgrades(type));

                addPointButtons[type].onClick.AddListener(
                    () => AddToUpgrades(type));
            }
        }

        private void SubtractFromUpgrades(AttributeType type)
        {
            if (!playerAttributes.TryAddToUpgrades(type, -1, out string failMsg))
            {
                upgradeMessagesPanel.SetActive(true);
                upgradeMessages.text = failMsg;

                if (messageTimer.IsStarted)
                {
                    messageTimer.Cancel();
                }

                messageTimer = new(this, 2f);
                messageTimer.Start();
            }
        }

        private void AddToUpgrades(AttributeType type)
        {
            if (!playerAttributes.TryAddToUpgrades(type, 1, out string failMsg))
            {
                SetUpgradeMessage(failMsg);
            }
            else
            {
                ClearUpgradeMessage();
            }
        }

        private void SetUpgradeMessage(string msg)
        {
            upgradeMessagesPanel.SetActive(true);
            upgradeMessages.text = msg;

            if (messageTimer.IsStarted)
            {
                messageTimer.Cancel();
            }

            messageTimer = new(this, 2f);
            messageTimer.Start();
        }

        private void ClearUpgradeMessage()
        {
            messageTimer.Cancel();
            upgradeMessagesPanel.SetActive(false);
        }
    }
}
