using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public enum TargetingEventType { CANCELLED, STARTED, CONFIRMED, EXECUTING, COMPLETED }
    public abstract class CombatAction
    {
        public readonly Sprite Icon;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;

        public readonly List<CombatSubactionSO> Subactions = new();
        public readonly List<CombatSubactionSO> DirectionBasedSubactions = new();
        public readonly List<CombatSubactionSO> FocusBasedSubactions = new();

        public readonly List<ISubactionCommand> Commands = new();

        private Coroutine executionProcess;

        private TargetSet directionBasedTargetSet = new();
        private TargetSet focusBasedTargetSet = new();
        private TargetSet cumulativeTargetSet = new();

        private bool registered;

        public event EventHandler<TargetingEventArgs> TargetingEvent;

        protected CombatAction(
            Sprite icon,
            List<CombatSubactionSO> subactions,
            AnimatorOverrideController overrideController,
            Combatant user)
        {
            Icon = icon;
            Subactions = subactions;
            OverrideController = overrideController;
            User = user;

            SortByPatternOrigin(
                Subactions,
                out DirectionBasedSubactions,
                out FocusBasedSubactions);
        }

        public void SubscribeToDirectionUpdates(Combatant user)
        {
            Debug.LogWarning($"{this} trying to register for tile updates");

            if (registered) { return; }

            user.FocusTileChanged += HandleFocusTileChanged;

            registered = true;
            Debug.LogWarning($"{this} registered for tile updates");
        }

        public void UnsubscribeToDirectionUpdates(Combatant user)
        {
            if (!registered) { return; }

            user.FocusTileChanged -= HandleFocusTileChanged;

            registered = false;
        }

        protected void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            UnTarget(focusBasedTargetSet);
            RecalculateFocusBasedTargets(args.directionContext);
            Target(focusBasedTargetSet);
        }

        protected void HandleDirectionChanged(
            object sender,
            DirectionChangedEventArgs args)
        {
            UnTarget(directionBasedTargetSet);
            RecalculateDirectionBasedTargets(args.newDirectionContext);
            Target(directionBasedTargetSet);
        }

        protected void SortByPatternOrigin(
            List<CombatSubactionSO> all,
            out List<CombatSubactionSO> directionBased,
            out List<CombatSubactionSO> focusBased)
        {
            directionBased = new();
            focusBased = new();

            foreach (CombatSubactionSO subactionSO in Subactions)
            {
                if (subactionSO.TargetingPattern.PatternOrigin == PatternOriginType.USER)
                {
                    directionBased.Add(subactionSO);
                }
                else
                {
                    focusBased.Add(subactionSO);
                }
            }
        }

        protected void RecalculateDirectionBasedTargets(DirectionContext directionContext)
        {
            directionBasedTargetSet?.Clear();

            DirectionBasedSubactions.ForEach(subaction
                => directionBasedTargetSet
                += subaction.TargetingPattern.GetTargets(directionContext));

            RecalculateCumulativeTargets();
        }

        protected void RecalculateFocusBasedTargets(DirectionContext directionContext)
        {
            focusBasedTargetSet?.Clear();

            FocusBasedSubactions.ForEach(subaction
                => focusBasedTargetSet
                += subaction.TargetingPattern.GetTargets(directionContext));

            RecalculateCumulativeTargets();
        }

        protected void RecalculateCumulativeTargets()
        {
            cumulativeTargetSet?.Clear();
            cumulativeTargetSet = directionBasedTargetSet + focusBasedTargetSet;
        }

        public void Equip()
        {
            /// Subscribe to direction updates
            SubscribeToDirectionUpdates(User);
        }

        public void Unequip()
        {
            UnsubscribeToDirectionUpdates(User);
            UnTarget(cumulativeTargetSet);
        }

        public void LockTargets()
        {
            OnTargetingEvent(TargetingEventType.CONFIRMED);
        }

        public void UnlockTargets()
        {
            SubscribeToDirectionUpdates(User);
            OnTargetingEvent(TargetingEventType.CANCELLED);
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

            OnTargetingEvent(TargetingEventType.EXECUTING);
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

            OnTargetingEvent(TargetingEventType.COMPLETED);

            PostExecution();
        }

        protected abstract void PreExecution();
        protected abstract void PostExecution();


        protected virtual void OnTargetingEvent(TargetingEventType eventType)
        {
            string eventMessage =
                $"{this} trying to raise an event\n" +
                $"Event is: ";

            if (TargetingEvent == null)
            {
                eventMessage += $"null.";
            }
            else
            {
                eventMessage +=
                    $"not null, and its subscriber count is:" +
                    $"{TargetingEvent.GetInvocationList().Length}";
            }
            Debug.LogWarning(eventMessage);


            TargetingEventArgs args = new(
                User,
                eventType,
                User.CurrentDirectionContext);

            TargetingEvent?.Invoke(this, args);
        }

        private void Target(TargetSet targets)
        {
            targets?.all?.ForEach(target => target.SubscribeTo(TargetingEvent));
            OnTargetingEvent(TargetingEventType.STARTED);
        }
        private void UnTarget(TargetSet targets)
        {
            targets?.all?.ForEach(target => target.UnsubscribeTo(TargetingEvent));
            OnTargetingEvent(TargetingEventType.CANCELLED);
        }
    }

    public class TargetingEventArgs : EventArgs
    {
        public Combatant user;
        public TargetingEventType eventType;
        public readonly DirectionContext directionContext;

        public TargetingEventArgs(
            Combatant user,
            TargetingEventType actionState,
            DirectionContext directionContext)
        {
            this.user = user;
            this.eventType = actionState;
            this.directionContext = directionContext;
        }
    }
}
