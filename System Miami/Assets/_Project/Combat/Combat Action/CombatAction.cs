using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public enum CombatActionEventType { CONFIRMED, EXECUTING, COMPLETED }
    public abstract class CombatAction
    {
        public readonly Sprite Icon;
        public readonly List<CombatSubaction> SubActions;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;

        private Coroutine executionProcess;

        private Targets cumulativeTargets;

        private bool registered;

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

            cumulativeTargets = new();
            RecalculateCumulativeTargets(User.CurrentDirectionContext);
            SubscribeCumulativeTargets();
        }

        public void RegisterForDirectionUpdates(Combatant user)
        {
            Debug.LogWarning($"{this} trying to register for tile updates");

            if (registered) { return; }

            foreach (CombatSubaction subaction in SubActions)
            {
                if (subaction.TargetingPattern.PatternOrigin == PatternOriginType.USER)
                {
                    Debug.LogWarning($"{this} trying to register for {subaction.TargetingPattern.PatternOrigin} tile updates");

                    user.DirectionChanged += HandleDirectionChanged;
                }
                else
                {
                    Debug.LogWarning($"{this} trying to register for {subaction.TargetingPattern.PatternOrigin} tile updates");
                    user.FocusTileChanged += HandleFocusTileChanged;
                }
            }

            registered = true;
            Debug.LogWarning($"{this} registered for tile updates");
        }

        public void DeregisterForDirectionUpdates(Combatant user)
        {
            if (!registered) { return; }

            foreach (CombatSubaction subaction in SubActions)
            {
                if (subaction.TargetingPattern.PatternOrigin == PatternOriginType.USER)
                {
                    user.DirectionChanged -= HandleDirectionChanged;
                }
                else
                {
                    user.FocusTileChanged -= HandleFocusTileChanged;
                }
            }

            registered = false;
        }

        protected void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            UnsubscribeCumulativeTargets();
            RecalculateCumulativeTargets(args.directionContext);
            SubscribeCumulativeTargets();
        }

        protected void HandleDirectionChanged(
            object sender,
            DirectionChangedEventArgs args)
        {
            UnsubscribeCumulativeTargets();
            RecalculateCumulativeTargets(args.newDirectionContext);
            SubscribeCumulativeTargets();
        }

        protected void SubscribeCumulativeTargets()
        {
            cumulativeTargets?.all?.ForEach(target => target.SubscribeTo(CombatActionEvent));
        }

        protected void UnsubscribeCumulativeTargets()
        {
            cumulativeTargets?.all?.ForEach(target => target.UnsubscribeTo(CombatActionEvent));
        }

        protected void RecalculateCumulativeTargets(DirectionContext newDirectionContext)
        {
            cumulativeTargets?.Clear();

            foreach(CombatSubaction subaction in SubActions)
            {
                Targets subactionTargs = subaction.TargetingPattern.GetTargets(newDirectionContext);

                subaction.IssueCommands(subactionTargs.all);

                cumulativeTargets += subactionTargs;
            }
        }

        public void Equip()
        {
        }

        public void Unequip()
        {
            
        }

        public void BeginConfirmingTargets()
        {
            OnActionEvent(CombatActionEventType.CONFIRMED);
        }

        public void BeginExecution()
        {
            executionProcess = User.StartCoroutine(Execute());
        }


        /// TODO: Implement this method
        public bool PlayerFoundInTargets()
        {
            return false;
        }

        protected IEnumerator Execute()
        {
            PreExecution();
            yield return null;

            OnActionEvent(CombatActionEventType.EXECUTING);
            yield return null;

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
            Debug.LogWarning($"{this} trying to raise an event");
            CombatActionEvent?.Invoke(this, new(User, eventType));
        }
    }

    public class CombatActionEventArgs : EventArgs
    {
        public Combatant user;
        public CombatActionEventType eventType;

        public CombatActionEventArgs(Combatant user, CombatActionEventType actionState)
        {
            Debug.LogWarning($"INSIDE CONSTRUCTOR of {this}");
            this.user = user;
            this.eventType = actionState;
        }
    }
}
