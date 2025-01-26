// Authors: Layla Hoey, Lee St. Louis
using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using SystemMiami.Enums;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Abilities/Ability")]
    public class Ability : ScriptableObject
    {
        [Header("Basic Info")]

        [SerializeField, Tooltip("Determines which type of slot this ability will occupy. Also determines the resource that will be used when the ability is executed.")]
        private AbilityType _type;

        /// <summary>
        /// Determines which Resource to consume when the ability is executed. Determined by the AbilityType.
        /// </summary>
        private ResourceType _requiredResource;
        
        [SerializeField, Tooltip("The amount of the required resource that will be consumed upon the ability being used once.")]
        private float _resourceCost;

        [SerializeField, Tooltip("The icon that will appear in the ui for this ability")]
        private Sprite _icon;
        public Sprite Icon { get { return _icon; } }


        [Header("Actions"), Space(5)]

        [SerializeField, Tooltip("An array of combat actions. The order of the array determines the execution order, as well as the order that the actions find and gather targets.")]
        private CombatSubaction[] _actions;


        [Header("Animation"), Space(5)]

        [SerializeField, Tooltip("The animation controller to override the combatant's when they perform this ability.")]
        public AnimatorOverrideController _overrideController;

        [Header("Cooldown")]

        [SerializeField, Tooltip("How many turns until they can use the ability again")]
        private int coolDownTurns;
        private int currentCooldown = 0;
        public bool isOnCooldown => currentCooldown > 0;

        [HideInInspector] public Combatant User;
        public AbilityType Type { get { return _type; } }
        public CombatSubaction[] Actions { get { return _actions; } }
        public bool IsBusy { get; private set; }

       // [SerializeField] AbilityDirections animDirs;

       // protected AnimationClipOverrides clipOverrides;

        public bool PlayerFoundInTargets
        {
            get
            {
                foreach (CombatSubaction action in _actions)
                {
                    List<Combatant> targets = action.TargetingPattern.StoredTargets.Combatants;

                    if (targets == null) { continue; }

                    Combatant player = targets.Find(c => c.StateMachine.IsPlayer);

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
        
            setResource();
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

            //do other stuff
        }


        public IEnumerator Use()
        {
            Resource resource = _requiredResource switch
            {
                ResourceType.STAMINA    => User.Stamina,
                ResourceType.MANA       => User.Mana,
                _                       => User.Stamina
            };

            resource.Lose(_resourceCost);

            yield return null;

            for (int i = 0; i < _actions.Length; i++)
            {
                _actions[i].Perform();

                _actions[i].TargetingPattern.UnlockTargets();
                _actions[i].TargetingPattern.HideTargets();
                //Debug.Log("Doing My actions");
            }

            currentCooldown = coolDownTurns;


            yield return null;
        }

        /// <summary>
        /// Assigns the _requiredResource based on _type
        /// </summary>
        private void setResource()
        {
            _requiredResource = _type switch
            {
                AbilityType.PHYSICAL => ResourceType.STAMINA,
                AbilityType.MAGICAL => ResourceType.MANA,
                _ => ResourceType.STAMINA,
            };
        }

        #region Cooldowns

        //reduce cooldown by one turn 
        public void ReduceCooldown()
        {
            if (currentCooldown > 0)
            {
                currentCooldown--;
            }
        }

        #endregion
    }

}
