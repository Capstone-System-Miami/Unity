using System;
using System.Collections;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class WaitThenDoTimer
    {
        private const string NONE = "Not running";
        private const string CANCELLED  = "Cancelled timer";

        /// <summary>
        /// The MonoBehaviour who will run
        /// the IEnumerator as a Coroutine.
        /// </summary>
        private readonly MonoBehaviour runner;

        /// <summary>
        /// The condition(s) the timer is waiting for.
        /// </summary>
        private readonly Conditions conditions;

        /// <summary>
        /// The action to perform when conditions are met.
        /// </summary>
        private readonly Action action;

        private Coroutine process;

        private bool BeenCancelled { get; set; }
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }

        /// <summary>
        /// If been started and not been cancelled,
        /// this will return a string
        /// <para>
        /// a) "Conditions not met."</para>
        /// <para>
        /// b) "Conditions met."</para>
        /// </summary>
        public string StatusMsg { get; private set; } = NONE;


        public WaitThenDoTimer(MonoBehaviour runner, Conditions conditions, Action action)
        {
            this.runner = runner;
            this.conditions = conditions;
            this.action = action;

            IsStarted = false;
            IsFinished = false;
            BeenCancelled = false;

            StatusMsg = NONE;
        }

        /// <summary>
        /// Start a timer <c>process</c>(<see cref="Coroutine"/>)
        /// using the <c>runner</c>(<see cref="MonoBehaviour"/>)
        /// that waits until <c>conditions</c>(<see cref="Conditions"/>)
        /// set during construction of this
        /// <see cref="WaitThenDoTimer"/> are met.
        /// </summary>
        public void Start()
        {
            if (BeenCancelled) { return; }

            IsStarted = true;
            IsFinished = false;

            process = runner.StartCoroutine(TimerProcess());
        }

        /// <summary>
        /// Render this <see cref="WaitThenDoTimer"/> unusable.
        /// Cancels the <see cref="Coroutine"/> <c>process</c>
        /// if it has been started.
        /// </summary>
        public void Cancel()
        {
            BeenCancelled = true;
            StatusMsg = CANCELLED;

            if (process == null) { return; }

            runner.StopCoroutine(process);
        }

        protected IEnumerator TimerProcess()
        {
            IsStarted = true;
            IsFinished = false;

            while (!conditions.AllMet())
            {
                string msg = $"Conditions not met.";
                if (StatusMsg != msg)
                {
                    StatusMsg = msg;
                }

                yield return null;
            }

            StatusMsg = $"Conditions met.";

            IsFinished = true;
            StatusMsg = NONE;
            yield return null;
        }
    }
}
