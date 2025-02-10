using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public enum TargetingEventType { CANCELLED, STARTED, LOCKED, EXECUTING, COMPLETED }

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

        public bool ExecutionStarted { get; private set; } = false;
        public bool ExecutionFinished { get; private set; } = false;

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
                out FocusBasedSubactions,
                out DirectionBasedSubactions);
        }

        public void SubscribeToDirectionUpdates(Combatant user)
        {
            Debug.LogWarning($"{this} trying to register for tile updates");

            if (registered) { return; }

            user.FocusTileChanged += HandleFocusTileChanged;
            user.DirectionChanged += HandleDirectionChanged;

            registered = true;
            Debug.LogWarning($"{this} registered for tile updates");
        }

        public void UnsubscribeToDirectionUpdates(Combatant user)
        {
            if (!registered) { return; }

            user.FocusTileChanged -= HandleFocusTileChanged;
            user.DirectionChanged -= HandleDirectionChanged;

            registered = false;
        }

        protected void HandleFocusTileChanged(
            object sender,
            FocusTileChangedEventArgs args)
        {
            UnTarget(focusBasedTargetSet);
            RecalculateFocusBasedTargets(args.directionContext);
            RecalculateCumulativeTargets();
            Target(focusBasedTargetSet);
        }

        protected void HandleDirectionChanged(
            object sender,
            DirectionChangedEventArgs args)
        {
            UnTarget(directionBasedTargetSet);
            RecalculateDirectionBasedTargets(args.newDirectionContext);
            RecalculateCumulativeTargets();
            Target(directionBasedTargetSet);
        }

        protected void SortByPatternOrigin(
            List<CombatSubactionSO> all,
            out List<CombatSubactionSO> focusBased,
            out List<CombatSubactionSO> directionBased)
        {
            focusBased = new();
            directionBased = new();

            foreach (CombatSubactionSO subactionSO in Subactions)
            {
                if (subactionSO.TargetingPattern.PatternOrigin == PatternOriginType.FOCUS)
                {
                    focusBased.Add(subactionSO);
                }
                else
                {
                    directionBased.Add(subactionSO);
                }
            }
        }

        protected void RecalculateFocusBasedTargets(DirectionContext directionContext)
        {
            focusBasedTargetSet?.Clear();

            FocusBasedSubactions.ForEach(subaction => focusBasedTargetSet
                += subaction.TargetingPattern.GetTargets(directionContext));
        }

        protected void RecalculateDirectionBasedTargets(DirectionContext directionContext)
        {
            directionBasedTargetSet?.Clear();

            DirectionBasedSubactions.ForEach(subaction => directionBasedTargetSet
                += subaction.TargetingPattern.GetTargets(directionContext));
        }

        protected void RecalculateCumulativeTargets()
        {
            cumulativeTargetSet?.Clear();
            cumulativeTargetSet = directionBasedTargetSet + focusBasedTargetSet;
        }

        public void Equip()
        {
            RecalculateFocusBasedTargets(User.CurrentDirectionContext);
            RecalculateDirectionBasedTargets(User.CurrentDirectionContext);
            RecalculateCumulativeTargets();

            Target(cumulativeTargetSet);

            SubscribeToDirectionUpdates(User);
        }

        public void Unequip()
        {
            UnsubscribeToDirectionUpdates(User);

            UnTarget(cumulativeTargetSet);
        }

        public void LockTargets()
        {
            UnTarget(cumulativeTargetSet);

            RecalculateFocusBasedTargets(User.CurrentDirectionContext);
            RecalculateDirectionBasedTargets(User.CurrentDirectionContext);
            RecalculateCumulativeTargets();

            Target(cumulativeTargetSet);

            UnsubscribeToDirectionUpdates(User);
            OnTargetingEvent(TargetingEventType.LOCKED);
        }

        public void UnlockTargets()
        {
            UnTarget(cumulativeTargetSet);
            cumulativeTargetSet.Clear();
            focusBasedTargetSet.Clear();
            directionBasedTargetSet.Clear();
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
            ExecutionStarted = true;
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
                Debug.Log($"AnimSim time remaining: {timer.StatusMsg()}");
                yield return null;
            } while (!timer.IsFinished);

            OnTargetingEvent(TargetingEventType.COMPLETED);

            PostExecution();
            ExecutionFinished = true;
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
            Debug.LogWarning("Starting subscription process...");
            targets?.all?.ForEach(target =>
            {
                Debug.LogWarning($"Subscribing {target} to TargetingEvent...");

                target.SubscribeTo(ref TargetingEvent);

                Debug.LogWarning(
                    $"Subscription complete for {target}." +
                    $"TargetingEvent is null: {TargetingEvent == null}");

                AssignCommands(target);
            });

            Debug.LogWarning("All subscriptions complete. Raising event...");

            OnTargetingEvent(TargetingEventType.STARTED);            
        }

        private void UnTarget(TargetSet targets)
        {
            OnTargetingEvent(TargetingEventType.CANCELLED);
            targets?.all?.ForEach(target =>
            {
                target.UnsubscribeTo( ref TargetingEvent);

                ClearCommands(target);
            });

        }

        private void AssignCommands(ITargetable target)
        {
            Subactions.ForEach(subaction =>
                target.TargetedBy.Add(subaction.GenerateCommand(target)));
        }

        private void ClearCommands(ITargetable target)
        {
            target.TargetedBy.Clear();
        }
    }

    public class TargetingEventArgs : EventArgs
    {
        public readonly Combatant User;
        public readonly TargetingEventType EventType;
        public readonly DirectionContext DirectionContext;

        public TargetingEventArgs(
            Combatant user,
            TargetingEventType eventType,
            DirectionContext directionContext)
        {
            User = user;
            EventType = eventType;
            DirectionContext = directionContext;
        }
    }
}
