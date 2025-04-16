using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace SystemMiami.CombatRefactor
{
    public enum TargetingEventType { CANCELLED, STARTED, LOCKED, EXECUTING, COMPLETED, REPORTBACK }

    public abstract class CombatAction
    {
        public readonly Sprite Icon;
        
        public readonly AnimatorOverrideController DefaultOverrideController;
        public readonly AnimatorOverrideController FighterOverrideController;
        public readonly AnimatorOverrideController MageOverrideController;
        public readonly AnimatorOverrideController TankOverrideController;
        public readonly AnimatorOverrideController RogueOverrideController;
        public readonly Combatant User;
        public readonly int ID;
        public readonly ItemType type;
        public readonly bool IsGeneralAbility;

        public readonly List<CombatSubactionSO> Subactions = new();
        public readonly List<CombatSubactionSO> DirectionBasedSubactions = new();
        public readonly List<CombatSubactionSO> FocusBasedSubactions = new();

        public readonly List<ISubactionCommand> Commands = new();

        public dbug log = new();

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
            int ActionID,
            List<CombatSubactionSO> subactions, AnimatorOverrideController defaultOverrideController,
            AnimatorOverrideController fighterOverrideController,AnimatorOverrideController mageOverrideController,
            AnimatorOverrideController tankOverrideController, AnimatorOverrideController rogueOverrideController,bool isGeneralAbility,
            Combatant user)
        {
            ID = ActionID;
            Icon = icon;
            Subactions = subactions;
            DefaultOverrideController = defaultOverrideController;
            FighterOverrideController = fighterOverrideController;
            MageOverrideController = mageOverrideController;
            TankOverrideController = tankOverrideController;
            RogueOverrideController = rogueOverrideController;
            IsGeneralAbility = isGeneralAbility;
            User = user;

            SortByPatternOrigin(
                Subactions,
                out FocusBasedSubactions,
                out DirectionBasedSubactions);

            log = new();
            log.on();
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

          //  Debug.Log("Player found in targets FALSE", User);
            return false;
        }

        public void Reportback()
        {
            OnTargetingEvent(TargetingEventType.REPORTBACK);
        }

        public void SubscribeToDirectionUpdates(Combatant user)
        {
            // Debug.LogWarning($"{this} trying to register for tile updates");

            if (registered) { return; }

            user.FocusTileChanged += HandleFocusTileChanged;
            user.DirectionChanged += HandleDirectionChanged;

            registered = true;
            // Debug.LogWarning($"{this} registered for tile updates");
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
                Assert.IsNotNull(subactionSO);
                Assert.IsNotNull(subactionSO.TargetingPattern);

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

            User.Animator.runtimeAnimatorController = ClassOverrideController();
            AnimatorStateInfo stateInfo = User.Animator.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo clipInfo = User.Animator.GetCurrentAnimatorClipInfo(0)[0];
            ForceReenterState();
            yield return null;

            Debug.Log("Current Clip time: " + clipInfo.clip.length.ToString());
            Debug.Log("Current Clip Info: " + clipInfo.clip.name);

            CountdownTimer timer = new(User, clipInfo.clip.length);
            timer.Start();

            yield return new WaitUntil(() => timer.IsStarted);

            string timeMsg =
                $"Time is {Time.time}\n";

            yield return new WaitUntil( () => timer.IsFinished );

            timeMsg += $"Time is {Time.time}";

            Debug.LogWarning(timeMsg);
            OnTargetingEvent(TargetingEventType.EXECUTING);
            yield return null;

            User.Animator.runtimeAnimatorController = temp;

            OnTargetingEvent(TargetingEventType.COMPLETED);
            yield return null;

            PostExecution();
            yield return null;

            ExecutionFinished = true;
        }

        private void ForceReenterState()
        {
            int tileDirHash = Animator.StringToHash("TileDir");
            int tileDir = User.Animator.GetInteger(tileDirHash);
            int tempTileDir = tileDir == 0 ? 7 : 0;
            User.Animator.SetInteger(tileDirHash, tempTileDir);
            User.Animator.SetInteger(tileDirHash, tileDir);
        }

        private AnimatorOverrideController ClassOverrideController()
        {
            CharacterClassType userClass = User.gameObject.GetComponent<Attributes>()._characterClass;
            if (IsGeneralAbility || this is Consumable)
            {
                switch (userClass)
                {
                    case CharacterClassType.MAGE:
                        Debug.Log($"User class is {userClass}, returning {MageOverrideController.name}");
                        return MageOverrideController;
                    case CharacterClassType.ROGUE:
                        Debug.Log($"User class is {userClass}, returning {RogueOverrideController.name}");
                        return RogueOverrideController;
                    case CharacterClassType.TANK:
                        Debug.Log($"User class is {userClass}, returning {TankOverrideController.name}");
                        return TankOverrideController;
                    case CharacterClassType.FIGHTER:
                        Debug.Log($"User class is {userClass}, returning {FighterOverrideController.name}");
                        return FighterOverrideController;
                    default:
                        Debug.Log($"User class is {userClass},Defaulting returning {TankOverrideController.name}");
                        return TankOverrideController;
                }
            }
            else
            {
                return DefaultOverrideController;
            }
        }

        protected abstract void PreExecution();
        protected abstract void PostExecution();


        protected virtual void OnTargetingEvent(TargetingEventType eventType)
        {
            // NOTE: Keep this. useful debug for any & all combat.
            //
            // string eventMessage =
            //     $"{this} trying to raise an event\n" +
            //     $"Event is: ";
            //
            // if (TargetingEvent == null)
            // {
            //     eventMessage += $"null.";
            // }
            // else
            // {
            //     eventMessage +=
            //         $"not null, and its subscriber count is:" +
            //         $"{TargetingEvent.GetInvocationList().Length}";
            // }
            // Debug.LogWarning(eventMessage);

            TargetingEventArgs args = new(
                User,
                eventType,
                User.CurrentDirectionContext);

            TargetingEvent?.Invoke(this, args);
        }

        private void Target(TargetSet targets)
        {
            // Debug.LogWarning("Starting subscription process...");
            targets?.all?.ForEach(target =>
            {
                // Debug.LogWarning($"Subscribing {target} to TargetingEvent...");
                target.SubscribeTo(ref TargetingEvent);

                // Debug.LogWarning(
                //     $"Subscription complete for {target}." +
                //     $"TargetingEvent is null: {TargetingEvent == null}");

                AssignCommands(target);
            });

            // Debug.LogWarning("All subscriptions complete. Raising event...");

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
