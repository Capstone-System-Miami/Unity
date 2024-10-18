// Authors: Layla Hoey
using System.Collections;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    // TODO
    // Incomplete, chaotic, and partially tested
    // Depends on the creation & refactoring of other scripts.
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
    public class Ability : ScriptableObject
    {
        [Header("Basic Info")] [SerializeField, Tooltip("Name of ability")]
        public string name;
        [SerializeField, Tooltip("Determines which type of slot this ability will occupy. Also determines the resource that will be used when the ability is executed.")]
        private AbilityType _type;

        [SerializeField, Tooltip("The icon that will appear in the ui for this ability")] private Sprite Icon;


        /// <summary>
        /// Determines which Resource to consume when the ability is executed. Determined by the AbilityType.
        /// </summary>
        private ResourceType _requiredResource;
        
        [SerializeField, Tooltip("The amount of the required resource that will be consumed upon the ability being used once.")]
        private float _resourceCost;


        [Header("Actions"), Space(5)]
        [SerializeField, Tooltip("An array of combat actions. The order of the array determines the execution order, as well as the order that the actions find and gather targets.")]
        private CombatAction[] _actions;
        private Targets[] _targets;



        [Header("Animation")]
        [SerializeField, Tooltip("The animation controller to override the combatant's when they perform this ability.")]
        private AnimatorOverrideController _overrideController;

        [Header("Cooldown")] [SerializeField, Tooltip("How many turns until they can use the ability again")]
        private int coolDownTurns;

        private int currentCooldown = 0;
        public bool isOnCooldown => currentCooldown > 0;
        public Combatant _user;

        public CombatAction[] Actions { get { return _actions; } }
       public bool IsBusy { get; private set; }
        public bool IsPreviewing { get; private set; }

        #region LaylaStuff

        

        
        private void Awake()
        {
            // Assigns the _requiredResource based on _type
            _requiredResource = _type switch
            {
                AbilityType.PHYSICAL    => ResourceType.STAMINA,
                AbilityType.MAGICAL     => ResourceType.MANA,
                _                       => ResourceType.STAMINA,
            };  
        }

        public void Init(Combatant user)
        {
            _user = user;
            // I don't know how the animator override controller works
            // but that stuff would (or could?) go here.
        }

        private bool showTilesOf(CombatAction action)
        {
            Targets targets = action.GetUpdatedTargets(_user.DirectionInfo);

            foreach (OverlayTile tile in targets.Tiles)
            {
                tile.Target(action.TargetedTileColor);
            }

            return true;
        }

        private bool showCombatantsOf(CombatAction action)
        {
            Targets targets = action.GetUpdatedTargets(_user.DirectionInfo);

            foreach (Combatant combatant in targets.Combatants)
            {
                combatant.Target(action.TargetedCombatantColor);
            }

            return true;
        }

        private bool hideTilesOf(CombatAction action)
        {
            Targets targets = action.StoredTargets;
            if(targets.Tiles == null) return true;

            foreach (OverlayTile tile in targets.Tiles)
            {
                tile.UnTarget();
            }

            return true;
        }

        private bool hideCombatantsOf(CombatAction action)
        {
            Targets targets = action.StoredTargets;
            if (targets.Combatants == null) return true;

            foreach (Combatant combatant in targets.Combatants)
            {
                combatant.UnTarget();
            }

            return true;
        }

        public IEnumerator ShowAllTargets()
        {
            IsPreviewing = true;
            if (IsBusy) { yield break; }

            IsBusy = true;
            yield return null;

            for (int i = 0; i < _actions.Length; i++)
            {
                yield return new WaitUntil(() => hideTilesOf(_actions[i]));
                yield return new WaitUntil(() => hideCombatantsOf(_actions[i]));
                yield return new WaitUntil(() => showTilesOf(_actions[i]));
                yield return new WaitUntil(() => showCombatantsOf(_actions[i]));
            }

            yield return new WaitForEndOfFrame();
            IsBusy = false;
        }

        public IEnumerator HideAllTargets()
        {
            if (IsBusy) { yield break; }

            IsBusy = true;
            yield return null;

            for (int i = 0; i < _actions.Length; i++)
            {
                yield return new WaitUntil(() => hideTilesOf(_actions[i]));
                yield return new WaitUntil(() => hideCombatantsOf(_actions[i]));
            }

            yield return new WaitForEndOfFrame();
            IsBusy = false;
            IsPreviewing = false;
        }

        public IEnumerator Use()
        {
            //if (isOnCooldown)
            //{
            //    Debug.Log($"{name} is on Cooldown for {currentCooldown} more turns");
            //    yield break;
            //}

           
            //if (!IsBusy)
            //{
            //    Debug.Log("I'm Busy");
            //    yield break;
            //}

            //IsBusy = true;
            yield return null;

            for (int i = 0; i < _actions.Length; i++)
            {
                Targets targets = _actions[i].GetUpdatedTargets(_user.DirectionInfo);

                _actions[i].Perform(targets);
                Debug.Log("Doing My actions");
                yield return new WaitUntil(() => hideTilesOf(_actions[i]));
                yield return new WaitUntil(() => hideCombatantsOf(_actions[i]));
                
            }

            currentCooldown = coolDownTurns;

            yield return new WaitForEndOfFrame();
          //  IsBusy = false;
        }
        #endregion

        #region StuffLeeAdded

        /// <summary>
        /// Updates the target preview based on the tile under the mouse cursor.
        /// </summary>
        /// <param name="targetTile">The tile under the mouse cursor.</param>
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
        //    Vector2Int userPosition = (Vector2Int)_user.CurrentTile.gridLocation;
        //    Vector2Int targetPosition = (Vector2Int)targetTile.gridLocation;

        //    DirectionalInfo newDirectionInfo = new DirectionalInfo(userPosition, targetPosition);

        //    // If the direction vector is zero, do not update targets
        //    if (newDirectionInfo.DirectionVec == Vector2Int.zero)
        //    {
        //        IsBusy = false;
        //        IsPreviewing = false;
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
        //    IsPreviewing = true;

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
