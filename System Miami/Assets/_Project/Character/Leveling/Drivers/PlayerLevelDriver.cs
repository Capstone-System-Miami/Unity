using System;
using System.Collections.Generic;
using System.Collections;
using SystemMiami.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace SystemMiami.Drivers
{
    public class PlayerLevelDriver : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Header("Level Panel")]
        [SerializeField] private TMP_Text currentLevel;
        [SerializeField] private TMP_Text currentXP;
        [SerializeField] private TMP_Text xpToNext;
        [SerializeField] private TMP_InputField xpInput;
        [SerializeField] private TMP_Text buttonText;

        [Header("Event Panel")]
        [SerializeField] GameObject eventPanel;
        [SerializeField] private LabeledField eventText;

        [SerializeField] private PlayerLevel playerLevel;

        private int xpToAdd = int.MinValue;

        private List<string> eventMessages = new();
        private int messageIndex;

        private void OnEnable()
        {
            if (playerLevel == null)
            {
                log.error($"var playerLevel has not been set in the inspector");
                return;
            }

            playerLevel.LevelUp += HandleLevelUp;
        }

        private void OnDisable()
        {
            playerLevel.LevelUp -= HandleLevelUp;
        }

        private void Start()
        {
            ClearEventMessages();
        }

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

        public void NextMessage()
        {
            if (eventMessages == null) { return; }
            if (!eventMessages.Any() ) { return; }
            
            // If increment makes the index out of bounds
            if (++messageIndex >= eventMessages.Count)
            {
                // Set index to last
                messageIndex = eventMessages.Count - 1;
            }

            // Set UI to the message at index
            eventText.Value.SetForeground(eventMessages[messageIndex]);
        }

        public void PrevMessage()
        {
            if (eventMessages == null) { return; }
            if (!eventMessages.Any()) { return; }

            // If decrement makes the index out of bounds
            if (--messageIndex < 0)
            {
                // Set index to first
                messageIndex = 0;
            }

            // Set UI to the message at index
            eventText.Value.SetForeground(eventMessages[messageIndex]);
        }

        public void ClearEventMessages()
        {
            messageIndex = 0;
            eventMessages.Clear();
            eventPanel.SetActive(false);
        }

        private void HandleLevelUp(int obj)
        {
            int thisMessasgeIndex = eventMessages.Count;

            eventMessages.Add(
                $"Message {thisMessasgeIndex}\n" +
                $"LevelUp event fired & recieved during frame {Time.frameCount}\n" +
                $"The new level passed with the event was {obj}\n");

            eventPanel.SetActive(true);
            NextMessage();
        }
    }
}
