using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items/RestoreResource")]
public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName;
        public Sprite icon;
        public String description;
        [Tooltip("The type of item it is")]
        public ItemType itemType;

        [Header("Actions"), Space(5)]

        [SerializeField, Tooltip("An array of combat actions. The order of the array determines the execution order, as well as the order that the actions find and gather targets.")]
        private CombatSubaction[] _actions;

        [Header("Animation"), Space(5)]

        [SerializeField, Tooltip("The animation controller to override the combatant's when they perform this ability.")]
        public AnimatorOverrideController _overrideController;

        [HideInInspector] public Combatant User;

        public CombatSubaction[] Actions { get { return _actions; } }

        public bool PlayerFoundInTargets
        {
            get
            {
                foreach (CombatSubaction action in _actions)
                {
                    List<Combatant> targets = action.TargetingPattern.StoredTargets.Combatants;

                    if (targets == null) { continue; }

                    Combatant player = targets.Find(c => c.Controller is PlayerController);
                    User = player;

                    if (player != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Init(Combatant user)
        {
            User = user;

        }

        public void BeginTargeting()
        {
            foreach (CombatSubaction action in _actions)
            {
                action.TargetingPattern.ClearTargets();
                action.TargetingPattern.UnlockTargets();
                action.TargetingPattern.ShowTargets();
                action.TargetingPattern.SubscribeToDirectionUpdates(User);
            }
        }

        public void CancelTargeting()
        {
            foreach (CombatSubaction action in _actions)
            {
                action.TargetingPattern.HideTargets();
                action.TargetingPattern.UnlockTargets();
                action.TargetingPattern.ClearTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }
        }

        /// <summary>
        /// Locks the targets by unsubscribing from moveDirection updates without hiding the targets.
        /// </summary>
        public void LockTargets()
        {
            foreach (CombatSubaction action in _actions)
            {
                action.TargetingPattern.LockTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }

        }


        public IEnumerator Use()
        {


            for (int i = 0; i < _actions.Length; i++)
            {
                _actions[i].Perform();


               _actions[i].TargetingPattern.UnlockTargets();
               _actions[i].TargetingPattern.HideTargets();

        }

            yield return null;
        }
    }

public enum ItemType
{
    Potion,
    EquipmentMod,
    Accesory,
    WeaponMod
}