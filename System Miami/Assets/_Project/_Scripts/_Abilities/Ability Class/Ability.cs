// Authors: Layla Hoey, Lee St. Louis
using System.Collections;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, chaotic, and partially tested
    // Depends on the creation & refactoring of other scripts.
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
        private Sprite Icon;


        [Header("Actions"), Space(5)]

        [SerializeField, Tooltip("An array of combat actions. The order of the array determines the execution order, as well as the order that the actions find and gather targets.")]
        private CombatAction[] _actions;


        [Header("Animation"), Space(5)]

        [SerializeField, Tooltip("The animation controller to override the combatant's when they perform this ability.")]
        public AnimatorOverrideController _overrideController;

        [Header("Cooldown")]

        [SerializeField, Tooltip("How many turns until they can use the ability again")]
        private int coolDownTurns;
        private int currentCooldown = 0;
        public bool isOnCooldown => currentCooldown > 0;

        [HideInInspector] public Combatant User;

        public CombatAction[] Actions { get { return _actions; } }
        public bool IsBusy { get; private set; }

        #region LaylaStuff

        public void Init(Combatant user)
        {
            User = user;
            // I don't know how the animator override controller works
            // but that stuff would (or could?) go here.

            setResource();
        }

        public void BeginTargeting()
        {
            foreach (CombatAction action in _actions)
            {
                action.TargetingPattern.SubscribeToDirectionUpdates(User);
            }
        }

        public void CancelTargeting()
        {
            foreach (CombatAction action in _actions)
            {
                action.TargetingPattern.UnlockTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }
        }

        /// <summary>
        /// Locks the targets by unsubscribing from moveDirection updates without hiding the targets.
        /// </summary>
        public void ConfirmTargets()
        {
            foreach (CombatAction action in _actions)
            {
                action.TargetingPattern.LockTargets();
                action.TargetingPattern.UnsubscribeToDirectionUpdates(User);
            }

            //do other stuff
        }


        public IEnumerator Use()
        {
            // TODO: Decrement resource


            yield return null;

            for (int i = 0; i < _actions.Length; i++)
            {
                _actions[i].Perform();

                _actions[i].TargetingPattern.UnlockTargets();
                _actions[i].TargetingPattern.HideTargets();
                Debug.Log("Doing My actions");             
            }

            currentCooldown = coolDownTurns;

            yield return new WaitForEndOfFrame();
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
        #endregion

        #region StuffLeeAdded

        /// <summary>
        /// Updates the target preview based on the tile under the mouse cursor.
        /// </summary>
        /// <param name = "targetTile" > The tile under the mouse cursor.</param>
        //public IEnumerator UpdateTargetPreview(OverlayTile targetTile)
        //{
        //    if (IsBusy)
        //    {
        //        yield break;
        //    }

        //    IsBusy = true;
        //    yield return null;

        //    //hide previous targets
        //    for (int i = 0; i < _actions.Length; i++)
        //    {
        //        yield return new WaitUntil(() => hideTilesOf(_actions[i]));
        //        yield return new WaitUntil(() => hideCombatantsOf(_actions[i]));
        //    }

        //    //create a new DirectionalInfo based on user pos and target tile
        //    Vector2Int userPosition = (Vector2Int)User.CurrentTile.gridLocation;
        //    Vector2Int targetPosition = (Vector2Int)targetTile.gridLocation;

        //    DirectionalInfo newDirectionInfo = new DirectionalInfo(userPosition, targetPosition);

        //    // If the moveDirection vector is zero, do not update targets
        //    if (newDirectionInfo.DirectionVec == Vector2Int.zero)
        //    {
        //        IsBusy = false;
        //        IsTargeting = false;
        //        yield break;
        //    }

        //    //update stored targets for each action
        //    for (int i = 0; i < _actions.Length; i++)
        //    {
        //        _actions[i].GetUpdatedTargets(newDirectionInfo);
        //    }

        //    // Show new targets
        //    for (int i = 0; i < _actions.Length; i++)
        //    {
        //        yield return new WaitUntil(() => showTilesOf(_actions[i]));
        //        yield return new WaitUntil(() => showCombatantsOf(_actions[i]));
        //    }

        //    yield return new WaitForEndOfFrame();
        //    IsBusy = false;
        //    IsTargeting = true;

        //}

        #endregion

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
