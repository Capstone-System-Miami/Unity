using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SystemMiami.CombatRefactor;
using System;
using SystemMiami.Management;
using SystemMiami.ui;

namespace SystemMiami
{
    public class DialogueWindow : MonoBehaviour
    {
        [Header("InternalRefs")]
        [SerializeField] private Image backgroundPanel;
        [SerializeField] private LabeledField messageField;
        [SerializeField] private Button closeWindowButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;
        [SerializeField, ReadOnly] private string invoker;
        [SerializeField, ReadOnly] private string client;

        [Header("Testing")]
        [SerializeField] private bool TRIGGER_openWindow;
        [SerializeField] private bool openOnStart = false;
        [SerializeField] private bool wrapStart = false;
        [SerializeField] private bool wrapEnd = false;
        [SerializeField] private bool allowCloseEarly = false;
        [SerializeField] private string explicitHeader;
        [SerializeField] private List<string> explicitMessageList;
        [SerializeField] private bool useExplicitMessages;

        private string header;
        private List<string> messages = new();

        private int messageIndex = 0;
        private string currentMessage;

        [field: SerializeField, ReadOnly]
        public bool IsRunning { get; private set; } = false;

        [field: SerializeField, ReadOnly]
        public bool BeenToLast { get; private set; } = false;

        private bool AtFirstIndex => messageIndex == 0;
        private bool AtLastIndex => messageIndex == messages?.Count - 1;
        private bool IndexIsGreater => messageIndex >= messages.Count;
        private bool IndexIsLess => messageIndex < 0;

        private bool ShowPrevButton => IsRunning && (wrapStart || !AtFirstIndex);
        private bool ShowNextButton => IsRunning && (wrapEnd || !AtLastIndex);
        private bool ShowCloseButton => IsRunning && (allowCloseEarly || BeenToLast);


        public void Start()
        {
            if (openOnStart)
            {
                OpenWindow();
            }
            else
            {
                CloseWindow();
            }
        }

        private void OnEnable()
        {
            SubscribeToEvents();
        }

        private void Update()
        {
            if (TRIGGER_openWindow)
            {
                OpenWindow();
                ResetTriggers();
                return;
            }

            if (!BeenToLast && AtLastIndex)
            {
                BeenToLast = true;
            }

            prevButton.gameObject.SetActive(ShowPrevButton);
            nextButton.gameObject.SetActive(ShowNextButton);
            closeWindowButton.gameObject.SetActive(ShowCloseButton);

            if (!IsRunning) { return; }

            UpdateMessage();
            SetHeader();
            SetBody();
        }

        public void OpenWindow()
        {
            OpenWindow(allowCloseEarly, header, messages.ToArray());
        }

        public void OpenWindow(bool allowCloseEarly, string header, string[] messages)
        {
            UI.MGR.StartDialogue(this, wrapStart, wrapEnd, allowCloseEarly, header, messages);
        }

        public void CloseWindow()
        {
            UI.MGR.FinishDialogue();
        }

        public void FillWith(string header, string[] messages)
        {
            this.header = header;
            this.messages = messages.ToList();
        }

        public void FillWith(string header, List<string> messages)
        {
            this.header = header;
            this.messages = messages;
        }

        public void NextMessage()
        {
            if (messages == null) { return; }
            if (!messages.Any()) { return; }

            messageIndex++;

            // If increment makes the index out of bounds
            if (IndexIsGreater)
            {
                if (wrapEnd)
                {
                    // Set index to last
                    WrapIndex();
                }
                else
                {
                    EndCycle();
                }
            }
        }


        public void PrevMessage()
        {
            if (messages == null) { return; }
            if (!messages.Any()) { return; }

            messageIndex--;

            // If decrement makes the index out of bounds
            if (IndexIsLess)
            {
                if (wrapStart)
                {
                    WrapIndex();
                }
                else
                {
                    messageIndex = 0;
                }
            }
        }

        public void ClearMessages()
        {
            messageIndex = 0;
            messages.Clear();
            header = "";
            currentMessage = "";
        }

        protected virtual void SubscribeToEvents()
        {
            UI.MGR.DialogueStarted += HandleDialogueStarted;
            UI.MGR.DialogueFinished += HandleDialogueFinished;
        }
        protected virtual void UnsubscribeToEvents()
        {
            UI.MGR.DialogueStarted -= HandleDialogueStarted;
            UI.MGR.DialogueFinished -= HandleDialogueFinished;
        }

        private void HandleDialogueStarted(object sender, DialogueEventArgs args)
        {
            invoker = sender.ToString();
            client =  args.client == null ? "NULL" : args.client.ToString();

            this.allowCloseEarly = args.allowCloseEarly;
            FillWith(args.header, args.messages);
            BeginCycle();
        }
        private void HandleDialogueFinished(object sender, EventArgs args)
        {
            invoker = invoker.Insert(0, "prev: ");
            client = client.Insert(0, "prev: ");

            EndCycle();
        }

        private void UpdateMessage()
        {
            if (!messages.Any()) { return; }

            currentMessage = messages[messageIndex];
        }

        private void SetHeader()
        {
            messageField.Label.SetForeground(header);
        }

        private void SetBody()
        {
            messageField.Value.SetForeground(currentMessage);
        }

        private void WrapIndex()
        {
            if (IndexIsLess)
            {
                messageIndex = messages.Count - 1;
            }
            else if (IndexIsGreater)
            {
                messageIndex = 0;
            }
        }

        private void BeginCycle()
        {
            backgroundPanel.enabled = true;
            messageField.Show();

            if (useExplicitMessages)
            {
                header = explicitHeader;
                messages = explicitMessageList;
            }

            if (header == null || header.Length == 0)
            {
                header = $"BeginCycle() called w/o header set";
            }
            
            if (messages.Count == 0)
            {
                messages = new List<string>
                {
                    $"error",
                    $"no messages set at the time ",
                    $"that BeginCycle() was called.",
                    $"(hi)",
                };
            }

            IsRunning = true;
            BeenToLast = false;
        }

        private void EndCycle()
        {
            ClearMessages();
            backgroundPanel.enabled = false;
            messageField.Hide();
            IsRunning = false;
        }

        private void ResetTriggers()
        {
            TRIGGER_openWindow = false;
        }

        private void OnDisable()
        {
            UnsubscribeToEvents();
        }
    }
}
