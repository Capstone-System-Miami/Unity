using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatAction
    {
        #region PUBLIC VARS
        //==============================
        public readonly Sprite Icon;
        public readonly List<CombatSubaction> SubActions;
        public readonly AnimatorOverrideController OverrideController;
        public readonly Combatant User;
        #endregion PUBLIC VARS

        #region PUBLIC METHODS
        //==============================

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

        public void UpdateDirection(
            DirectionContext newDirection,
            bool directionChanged)
        {
            SubActions.ForEach(subaction
                => subaction.UpdateTargets(newDirection, directionChanged));
        }

        public void Unequip()
        {
            SubActions.ForEach(subaction
                => subaction.ClearTargets());
        }

        /// TODO: Implement this method
        public bool PlayerFoundInTargets()
        {
            return false;
        }

        public abstract IEnumerator Execute();
        #endregion PUBLIC METHODS

        #region PROTECTED METHODS
        //==============================
        protected void performActions()
        {
            SubActions.ForEach(subaction => subaction.Perform());
        }
        #endregion PROTECTED METHODS
    }
}
