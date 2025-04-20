using System.Collections.Generic;
using System.Collections;
using System.Linq;
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

        private int currentIndex = 0;
        private int finalIndex => pressInSequence?.Count - 1 ?? 0;

        private MonoBehaviour runner;
        private Coroutine process;
        private Action monoAction;

        public event Action codeEntered;

        public Coroutine Start(MonoBehaviour runner, Action action)
        {
            Debug.Log("TOP of Start fn");
            if (this.runner != null && this.runner != runner)
            {
                Debug.LogWarning(
                    $"This dev key command is already owned by " +
                    $"{this.runner.name}. Can't assign additional " +
                    $"actions from {runner.name}...");
                return process;
            }

            this.runner = runner;
            monoAction = action;
            string name = $"{this.runner.name}'s DevKeycode";

            if (process != null)
            {
                Debug.LogWarning(
                    $"Already checking for {name}. Returning existing process.");
                return process;
            }

            codeEntered += monoAction;
            codeEntered += () => SayHi(name);

            process = runner.StartCoroutine(CheckSequence());
            return process;
        }

        public string SayHi(string who)
        {
            return $"{who} says \"hi\".";
        }

        private IEnumerator CheckSequence()
        {
            currentIndex = 0;

            while (isHolding)
            {
                Debug.Log("HOLD");

                KeyCode current = pressInSequence[currentIndex];

                if (Input.anyKeyDown && !Input.GetKeyDown(current))
                {
                    currentIndex = 0;
                    yield return null;
                    continue;
                }
                else if (Input.GetKeyDown(current))
                {
                    if (++currentIndex > finalIndex)
                    {
                        codeEntered?.Invoke();
                        break;
                    }
                }

                yield return null;
            }

            Debug.Log("BOTTOM of check sequence");
            runner.StopCoroutine(this.process);
            this.process = null;
            runner = null;
            codeEntered = null;
        }
    }
}
