using System;
using System.Collections;
using UnityEngine;

// Authors: Layla
namespace SystemMiami.Utilities
{
    public class CountdownTimer
    {
        private const string NONE = "Not running";
        private const string CANCELLED  = "Cancelled timer";

        /// <summary>
        /// The MonoBehaviour who will run
        /// the IEnumerator as a Coroutine.
        /// </summary>
        private readonly MonoBehaviour runner;

        /// <summary>
        /// The amount of seconds the timer
        /// will run for.
        /// </summary>
        private readonly float seconds;

        private Coroutine process;

        private bool BeenCancelled { get; set; }
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }

        /// <summary>
        /// This will return a string
        /// <para>
        /// a) "Not running"</para>
        /// <para>
        /// b) "Cancelled"</para>
        /// <para>
        /// c) A string for seconds remaining,
        /// formatted as two digits</para>
        /// 
        /// Making this a <see cref="Func{TResult}"/>
        /// ensures that the rounding calc and
        /// string creation will only happen
        /// if something tries to read this property.
        /// on the timer.
        /// </summary>
        public string StatusMsg { get; private set; } = NONE;


        public CountdownTimer(MonoBehaviour runner, float seconds)
        {
            this.runner = runner;
            this.seconds = seconds;

            IsStarted = false;
            IsFinished = false;
            BeenCancelled = false;

            StatusMsg = NONE;
        }

        /// <summary>
        /// Start a timer <c>process</c>(<see cref="Coroutine"/>)
        /// using the <c>runner</c>(<see cref="MonoBehaviour"/>)
        /// for the number of <c>seconds</c>(<see cref="float"/>)
        /// set during construction of this
        /// <see cref="CountdownTimer"/>.
        /// </summary>
        public void Start()
        {
            if (BeenCancelled) { return; }

            IsStarted = true;
            IsFinished = false;

            process = runner.StartCoroutine(TimerProcess());
        }

        /// <summary>
        /// Render this <see cref="CountdownTimer"/> unusable.
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

            float remaining = seconds;
            int remainingRounded() => Mathf.RoundToInt(remaining);

            while (remaining >= 0)
            {
                string msg = $"Remaining; {remainingRounded():00}";
                if (StatusMsg != msg)
                {
                    StatusMsg = msg;
                }

                float frame = Time.deltaTime;

                remaining -= frame;                  

                yield return new WaitForSeconds(frame);
            }

            IsFinished = true;
            StatusMsg = NONE;
            yield return null;
        }
    }    
}
