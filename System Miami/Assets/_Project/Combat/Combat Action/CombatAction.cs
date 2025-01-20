using System.Collections;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatAction
    {
        #region PUBLIC VARS
        //==============================
        public readonly Sprite Icon;
        public readonly CombatSubaction[] Actions;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;
        #endregion PUBLIC VARS

        #region PUBLIC METHODS
        //==============================

        protected CombatAction(
            Sprite icon,
            CombatSubaction[] actions,
            AnimatorOverrideController overrideController,
            Combatant user)
        {
            Icon = icon;
            Actions = actions;
            OverrideController = overrideController;
            User = user;
        }

        public void BeginTargeting()
        {
            foreach (CombatSubaction action in Actions)
            {
                action.TargetingPattern.ClearTargets();
                action.TargetingPattern.UnlockTargets();
                action.TargetingPattern.ShowTargets();
                action.TargetingPattern.SubscribeToDirectionUpdates(User);
            }
        }

        public void CancelTargeting()
        {
            foreach (CombatSubaction action in Actions)
            {
                action.TargetingPattern.HideTargets();
                action.TargetingPattern.UnlockTargets();
                action.TargetingPattern.ClearTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }
        }

        /// TODO: Implement this method
        public bool PlayerFoundInTargets()
        {
            return false;
        }

        public void LockTargets()
        {
            foreach (CombatSubaction action in Actions)
            {
                action.TargetingPattern.LockTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }
        }

        public abstract IEnumerator Use();
        #endregion PUBLIC METHODS

        #region PROTECTED METHODS
        //==============================
        protected void performActions()
        {
            for (int i = 0; i < Actions.Length; i++)
            {
                Actions[i].Perform();

                Actions[i].TargetingPattern.UnlockTargets();
                Actions[i].TargetingPattern.HideTargets();
                //Debug.Log($"{this} is performing a subaction, {Actions[i]}.");
            }
        }
        #endregion PROTECTED METHODS
    }
}
