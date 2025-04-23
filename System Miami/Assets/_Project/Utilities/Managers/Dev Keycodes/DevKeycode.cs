using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

namespace SystemMiami.Utilities
{
    [Serializable]
    public class DevKeycode
    {
        [SerializeField] private List<KeyCode> holdDown = new();
        [SerializeField] private List<KeyCode> pressInSequence = new();

        public bool isHolding {
            get
            {
                foreach(KeyCode key in holdDown)
                {
                    if (!Input.GetKey(key)) return false;
                }
                return true;
            }
        }

        private const float cooldown = 1f;
        private float cooldownRemaining = 0f;
        private int currentIndex = 0;
        private int finalIndex => pressInSequence?.Count - 1 ?? 0;

        private MonoBehaviour runner;
        private Coroutine updateProcess;
        private Coroutine process;
        private Action monoAction;

        public event Action codeEntered;

        public void HookIn(MonoBehaviour runner, Action action)
        {
            // Debug.Log("TOP of Start fn");
            if (this.runner != null && this.runner != runner)
            {
                // Debug.LogWarning(
                //     $"This dev key command is already owned by " +
                //     $"{this.runner.name}. Can't assign additional " +
                //     $"actions from {runner.name}...");
                return;
            }

            this.runner = runner;
            monoAction = action;
            string name = $"{this.runner.name}'s DevKeycode";

            if (process != null)
            {
                // Debug.LogWarning(
                //     $"Already checking for {name}. Returning without modfying process.");
                return;
            }

            codeEntered += monoAction;

            currentIndex = 0;
            updateProcess = runner.StartCoroutine(UpdateProcess());
        }

        public void Unhook(MonoBehaviour runner)
        {
            if (this.runner != runner)
            {
                // Wrong runner
                return;
            }

            if (process != null)
            {
                runner.StopCoroutine(this.process);
                this.process = null;
            }

            codeEntered = null;
        }

        private void LocalUpdate()
        {
            if (!isHolding || pressInSequence.Count == 0) return;
            if (cooldownRemaining > 0)
            {
                cooldownRemaining -= Time.deltaTime;
                return;
            }

            // Debug.Log("HOLD");
            KeyCode current = pressInSequence[currentIndex];

            if (Input.anyKeyDown && !Input.GetKeyDown(current))
            {
                currentIndex = 0;
            }
            else if (Input.GetKeyDown(current)
                    && (++currentIndex > finalIndex) )
            {
                cooldownRemaining = cooldown;
                currentIndex = 0;
                codeEntered?.Invoke();
            }
        }

        private IEnumerator UpdateProcess()
        {
            while (runner != null)
            {
                LocalUpdate();
                yield return null;
            }
        }

        private IEnumerator CheckSequence()
        {
            yield break;
            currentIndex = 0;

            while (isHolding)
            {
            }

            // Debug.Log("BOTTOM of check sequence");
        }
    }
}
