using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public enum CombatActionEventType { UNEQUIPPED, EQUIPPED, CONFIRMED, EXECUTING, COMPLETED }
    public abstract class CombatAction
    {
        public readonly Sprite Icon;
        public readonly List<CombatSubaction> SubActions;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;

        private Coroutine executionProcess;

        public event EventHandler<CombatActionEventArgs> CombatActionEvent;

        protected CombatAction(
            Sprite icon,
            List<CombatSubaction> subActions,
            AnimatorOverrideController overrideController,
            Combatant user)
        {
            Icon = icon;
            SubActions = subActions;
            OverrideController = overrideController;
            User = user;
        }

        public void RegisterSubactions()
        {
            Debug.Log($"{this} made it to registration");
            SubActions.ForEach(subaction
                => subaction.RegisterForActionUpdates(this));
        }

        public void DeregisterSubactions()
        {
            SubActions.ForEach(subaction
                => subaction.DeregisterForActionUpdates(this));
        }

        public void Equip()
        {
            OnActionEvent(CombatActionEventType.EQUIPPED);
        }

        public void Unequip()
        {
            OnActionEvent(CombatActionEventType.UNEQUIPPED);
        }

        public void BeginConfirmingTargets()
        {
            OnActionEvent(CombatActionEventType.CONFIRMED);
        }

        public void BeginExecution()
        {
            OnActionEvent(CombatActionEventType.EXECUTING);
        }


        /// TODO: Implement this method
        public bool PlayerFoundInTargets()
        {
            return false;
        }

        // TODO:
        // (layla question)
        // Does this need to be an IEnumerator?
        // It feels like it could just be a System.Action
        // or other delegate type
        protected IEnumerator Execute()
        {
            PreExecution();
            yield return null;

            foreach (CombatSubaction subaction in SubActions)
            {
                subaction.Perform();
                yield return null;
            }

            CountdownTimer timer = new(User, 2f);
            timer.Start();

            yield return new WaitUntil(() => timer.IsStarted);
            Debug.Log($"An ability is starting an Anim simulation timer");

            do
            {
                Debug.Log($"AnimSim time remaining: {timer.StatusMsg}");
                yield return null;
            } while (!timer.IsFinished);

            OnActionEvent(CombatActionEventType.COMPLETED);

            PostExecution();
        }
        protected abstract void PreExecution();
        protected abstract void PostExecution();

        protected virtual void OnActionEvent(CombatActionEventType eventType)
        {
            CombatActionEvent?.Invoke(this, new(User, eventType));
        }
    }

    public class CombatActionEventArgs : EventArgs
    {
        public Combatant user;
        public CombatActionEventType eventType;

        public CombatActionEventArgs(Combatant user, CombatActionEventType actionState)
        {
            this.user = user;
            this.eventType = actionState;
        }
    }
}
