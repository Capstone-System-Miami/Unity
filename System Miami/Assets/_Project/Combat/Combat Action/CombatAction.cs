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


        /// TODO: Implement this method
        public bool PlayerFoundInTargets()
        {
            return false;
        }

        public abstract IEnumerator Execute();

        protected void PerformActions()
        {
            SubActions.ForEach(subaction => subaction.Perform());
        }

        protected virtual void OnActionEvent(CombatActionEventType eventType)
        {
            Debug.LogWarning($"{this} trying to invoke a {eventType}");

            CombatActionEvent?.Invoke(this, new(User, eventType));
        }
    }

    public class CombatActionEventArgs : EventArgs
    {
        public Combatant user;
        public CombatActionEventType actionState;

        public CombatActionEventArgs(Combatant user, CombatActionEventType actionState)
        {
            this.user = user;
            this.actionState = actionState;
        }
    }
}
