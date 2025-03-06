using System.Collections;
using System.Collections.Generic;
using SystemMiami.Utilities;
using TMPro;
using UnityEngine;

namespace SystemMiami.ui
{
    public class AttributePanel : MonoBehaviour
    {
        [SerializeField] dbug log;


        [Header("Info Text")]
        //[SerializeField] private GameObject upgradeMessagesPanel;
        //[SerializeField] private TMP_Text upgradeMessages;
        [SerializeField] private LabeledField attributePointsField;

        [SerializeField] private SinglePressButton upgradeModeButton;
        [SerializeField] private GameObjectToggler upgradeModeObjects;

        [Header("Attribute Set Visualizers")]
        [SerializeField] private AttributeSetVisualizer currentVisualizer;
        [SerializeField] private AttributeSetVisualizer previewVisualizer;

        [Header("Add & Subtract buttons")]
        [Space(5)]
        [SerializeField] private SinglePressButton addStr;
        [SerializeField] private SinglePressButton subtractStr;
        [Space(5)]
        [SerializeField] private SinglePressButton addDex;
        [SerializeField] private SinglePressButton subtractDex;
        [Space(5)]
        [SerializeField] private SinglePressButton addCon;
        [SerializeField] private SinglePressButton subtractCon;
        [Space(5)]
        [SerializeField] private SinglePressButton addWis;
        [SerializeField] private SinglePressButton subtractWis;
        [Space(5)]
        [SerializeField] private SinglePressButton addInt;
        [SerializeField] private SinglePressButton subtractInt;

        private Attributes playerAttributes;

        private bool isUpgradeMode;
        private bool upgradesEnabled => (playerAttributes.PointsRemaining) > 0;

        private AttributeSet Current => playerAttributes.CurrentCopy;
        private AttributeSet Upgrade => playerAttributes.UpgradesCopy;
        private AttributeSet Preview => playerAttributes.PreviewCopy;

        private Dictionary<AttributeType, SinglePressButton> subtractPointButtons = new();
        private Dictionary<AttributeType, SinglePressButton> addPointButtons = new();

        private CountdownTimer messageTimer;

        private void Awake()
        {

            //messageTimer = new(this, 2f);
        }

        private void Start()
        {
            playerAttributes = PlayerManager.MGR.GetComponent<Attributes>();

            RefreshValues();

            FillDictionaries();

            CancelUpgrade();

            //ClearUpgradeMessage();
        }

        private void Update()
        {
            attributePointsField.Value.SetForeground(
                $"{playerAttributes.PointsRemaining}");

            if (upgradesEnabled && !upgradeModeButton.gameObject.activeSelf)
            {
                upgradeModeButton.gameObject.SetActive(true);
            }
            else if (!upgradesEnabled && !isUpgradeMode && upgradeModeButton.gameObject.activeSelf)
            {
                upgradeModeButton.gameObject.SetActive(false);
            }

            //if (messageTimer != null && messageTimer.IsFinished)
            //{
            //    ClearUpgradeMessage();
            //}

            RefreshValues();
        }

        public void EnterUpgradeMode()
        {
            currentVisualizer.gameObject.SetActive(true);
            previewVisualizer.gameObject.SetActive(true);

            playerAttributes.EnterUpgradeMode();

            upgradeModeObjects.AllOn();

            isUpgradeMode = true;
        }

        public void CancelUpgrade()
        {
            currentVisualizer.gameObject.SetActive(true);
            previewVisualizer.gameObject.SetActive(false);

            playerAttributes.LeaveUpgradeMode();

            upgradeModeObjects.AllOff();

            isUpgradeMode = false;
        }

        public void ConfirmUpgrade()
        {
            currentVisualizer.gameObject.SetActive(true);
            previewVisualizer.gameObject.SetActive(false);

            playerAttributes.ConfirmUpgrades();

            upgradeModeObjects.AllOff();

            isUpgradeMode = false;
        }

        public void RefreshValues()
        {
            currentVisualizer.Assign(Current);
            previewVisualizer.Assign(Preview);
        }

        private void FillDictionaries()
        {
            // Define dictionaries
            subtractPointButtons = new Dictionary<AttributeType, SinglePressButton>
            {
                { AttributeType.STRENGTH, subtractStr },
                { AttributeType.DEXTERITY, subtractDex },
                { AttributeType.CONSTITUTION, subtractCon },
                { AttributeType.WISDOM, subtractWis },
                { AttributeType.INTELLIGENCE, subtractInt },
            };

            addPointButtons = new Dictionary<AttributeType, SinglePressButton>
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

                subtractPointButtons[type].AdditionalOnPointerDown.AddListener(
                    () => SubtractFromUpgrades(type));

                addPointButtons[type].AdditionalOnPointerDown.AddListener(
                    () => AddToUpgrades(type));
            }
        }

        private void SubtractFromUpgrades(AttributeType type)
        {
            if (!playerAttributes.TryAddToUpgrades(type, -1, out string failMsg))
            {
                //upgradeMessagesPanel.SetActive(true);
                //upgradeMessages.text = failMsg;

                //if (messageTimer.IsStarted)
                //{
                //    messageTimer.Cancel();
                //}

                //messageTimer = new(this, 2f);
                //messageTimer.Start();
            }
        }

        private void AddToUpgrades(AttributeType type)
        {
            if (!playerAttributes.TryAddToUpgrades(type, 1, out string failMsg))
            {
                //SetUpgradeMessage(failMsg);
            }
            else
            {
                //ClearUpgradeMessage();
            }
        }

        //private void SetUpgradeMessage(string msg)
        //{
        //    upgradeMessagesPanel.SetActive(true);
        //    upgradeMessages.text = msg;

        //    if (messageTimer.IsStarted)
        //    {
        //        messageTimer.Cancel();
        //    }

        //    messageTimer = new(this, 2f);
        //    messageTimer.Start();
        //}

        //private void ClearUpgradeMessage()
        //{
        //    messageTimer.Cancel();
        //    upgradeMessagesPanel.SetActive(false);
        //}
    }
}
