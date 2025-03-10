using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public enum TargetingEventType { CANCELLED, STARTED, LOCKED, EXECUTING, COMPLETED, REPORTBACK }

    public abstract class CombatAction
    {
        public readonly Sprite Icon;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;
        public readonly int ID;
        public readonly ItemType type;

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
            Sprite icon,int ActionID,
            List<CombatSubactionSO> subactions,
            AnimatorOverrideController overrideController,
            Combatant user)
        {
            ID = ActionID;
            Icon = icon;
            Subactions = subactions;
            OverrideController = overrideController;
            User = user;

            SortByPatternOrigin(
                Subactions,
                out FocusBasedSubactions,
                out DirectionBasedSubactions);
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

            focusBasedTargetSet.Clear();
            directionBasedTargetSet.Clear();
            cumulativeTargetSet.Clear();
        }

        public void LockTargets()
        {
            UnsubscribeToDirectionUpdates(User);
            OnTargetingEvent(TargetingEventType.LOCKED);
        }

        public void UnlockTargets()
        {
            Unequip();
            Equip();
        }

        public void BeginExecution()
        {
            executionProcess = User.StartCoroutine(Execute());
        }

        public void CleanUp()
        {
            Debug.Log(
                "Cleaning up CombatAction after execution.",
                User);
            if (this == null) return; 
            Unequip();
            ExecutionStarted = false;
            ExecutionFinished = false;
            if (executionProcess != null)
            {
                Debug.LogWarning(
                    "ExecutionProcess was not null on cleanup." +
                    "Stopping coroutine and deleting.",
                    User);

                User.StopCoroutine(executionProcess);
                executionProcess = null;
            }
        }


        public bool PlayerFoundInTargets()
        {
            foreach (ITargetable target in cumulativeTargetSet.all)
            {
                if (target is PlayerCombatant)
                {
                    Debug.Log("Player found in targets TRUE", User);
                    return true;
                }
            }

            Debug.Log("Player found in targets FALSE", User);
            return false;
        }

        public void Reportback()
        {
            OnTargetingEvent(TargetingEventType.REPORTBACK);
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



        protected IEnumerator Execute()
        {
            ExecutionStarted = true;
            PreExecution();
            yield return null;

            RuntimeAnimatorController temp = User.Animator.runtimeAnimatorController;

            User.Animator.runtimeAnimatorController = OverrideController;

            CountdownTimer timer = new(User, 2f);
            timer.Start();

            yield return new WaitUntil(() => timer.IsStarted);

            string timeMsg =
                $"An action is starting an Anim simulation timer, " +
                $"time remaining is {timer.StatusMsg}";
            do
            {
                if (timeMsg != timer.StatusMsg)
                {
                    timeMsg = timer.StatusMsg;
                    Debug.LogWarning(timeMsg);
                }
                yield return null;
            } while (!timer.IsFinished);

            OnTargetingEvent(TargetingEventType.EXECUTING);
            yield return null;

            User.Animator.runtimeAnimatorController = temp;

            OnTargetingEvent(TargetingEventType.COMPLETED);
            yield return null;

            PostExecution();
            yield return null;

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
            if (targets == null ) return;
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
                target.TargetedBy.Add(subaction.GenerateCommand(target, this)));
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
