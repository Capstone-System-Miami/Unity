using UnityEngine;
using System;
using UnityEngine.Events;
using SystemMiami.Utilities;

namespace SystemMiami
{
    public class SinglePressButton : BetterButton
    {
        [SerializeField] protected UnityEvent ButtonClickedEvent;
        //[SerializeField] private float pressDuration = 0.1f;

        protected override Action ClickStrategy => OnButtonClicked;

        protected void OnButtonClicked()
        {
            ButtonClickedEvent.Invoke();
        }

        // TODO: Test this
        //private void WaitThenClick()
        //{
        //    CountdownTimer timer = new(this, pressDuration);
        //    timer.Start();

        //    Conditions waitForTimer = new();
        //    waitForTimer.Add(() => timer.IsFinished);

        //    void buttonClickedDelegate() {
        //        ButtonClicked.Invoke();
        //    }

        //    WaitThenDoTimer waitThenInvoke = new(this, waitForTimer, buttonClickedDelegate);
        //}
    }
}
