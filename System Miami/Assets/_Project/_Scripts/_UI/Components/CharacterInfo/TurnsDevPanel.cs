using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class TurnsDevPanel : MonoBehaviour
    {
        // Vars
        #region Serialized
        [SerializeField] private TextBox _panelLabel;

        [SerializeField] private LabeledField _combatantField;

        [SerializeField] private LabeledField _phaseField;
        [SerializeField] private Color _movementColor;
        [SerializeField] private Color _actionColor;

        [SerializeField] TextBox _promptsBox;
        private string _movementText;
        private string _actionText;
        [SerializeField] private Color _promptsTextColor = Color.white;
        #endregion

        #region Private
        /// <summary>
        /// This script will break if this variable
        /// is changed directly. It is a backing field
        /// for the turnOwner Property and should
        /// should only be modified that way.
        /// </summary>
        private Combatant _turnOwner;
        #endregion

        #region Properties
        private Combatant turnOwner
        {
            get
            {
                return _turnOwner;
            }

            set
            {
                if (value == null) { return; }
                if (_turnOwner == value) { return; }

                if (_turnOwner != null)
                {
                    unsubscribeToAbilities(_turnOwner);
                }

                subscribeToAbilities(value);

                _turnOwner = value;
            }
        }
        #endregion

        #region Bools
        private bool _promptsEnabled;
        private bool _overrideActionPrompt;
        #endregion


        // Methods
        #region Unity
        private void Start() 
        {
            _panelLabel.SetForeground("Turn Indicator Dev Panel");
            _combatantField.Label.SetForeground("Combatant:");
            _phaseField.Label.SetForeground("Phase:");
        }

        private void Update()
        {
            turnOwner = TurnManager.MGR.CurrentTurnOwner;

            updateCombatantPanel();
            updatePhasePanel();

            if (TurnManager.MGR.IsPlayerTurn)
            {
                enablePrompts();
                updatePromptsPanel();
            }
            else
            {
                disablePrompts();
            }
        }
        #endregion


        #region Panel Updates
        private void updateCombatantPanel()
        {
            _combatantField.Value.SetForeground($"{turnOwner.name}");
            _combatantField.Value.SetForeground(turnOwner.ColorTag);
        }

        private void updatePhasePanel()
        {
            Phase currentPhase = turnOwner.Controller.CurrentPhase;

            Color phaseColor = currentPhase switch
            {
                Phase.Movement => _movementColor,
                Phase.Action => _actionColor,
                Phase.None => Color.black,
                _ => Color.black
            };

            _phaseField.Value.SetForeground($"{turnOwner.Controller.CurrentPhase}");
            _phaseField.Value.SetForeground(phaseColor);
        }

        private void updatePromptsPanel()
        {
            _movementText = getMovementPrompt();

            if (!_overrideActionPrompt)
            {               
                _actionText = getActionPrompt();
            }

            string prompt = turnOwner.Controller.CurrentPhase switch
            {
                Phase.Movement  => _movementText,
                Phase.Action    => _actionText,
                Phase.None      => _movementText,
                _               => _movementText
            };

            _promptsBox.SetForeground(prompt);
            _promptsBox.SetForeground(_promptsTextColor);
        }

        private string getMovementPrompt()
        {
            string result = "";

            if (turnOwner.Controller.CanMove)
            {
                result += $"Click a tile to move,\n\n";
            }
            else if (turnOwner.Speed.Get() > 0)
            {
                result += $"Moving To Tile.\n\n";
            }
            else
            {
                result += $"Speed Depleted\n\n";
            }

            result += $"Press E to end Movement Phase\n\n" +
                        $"Or Press Q to end Turn.";

            return result;
        }

        private string getActionPrompt()
        {
            string result = "";

            if (turnOwner.Controller.CanAct)
            {
                result += $"Click an Ability to Equip it,\n\n" +
                    $"Or ";
            }

            result += $"Press Q to End Turn.";

            return result;
        }

        private void enablePrompts()
        {
            if (_promptsEnabled) { return; }

            _promptsBox.ShowBackground();
            _promptsBox.ShowForeground();
        }

        private void disablePrompts()
        {
            if (!_promptsEnabled) { return; }

            _promptsBox.HideBackground();
            _promptsBox.HideForeground();
        }
        #endregion


        #region Ability Subscriptions
        private void subscribeToAbilities(Combatant combatant)
        {
            combatant.Abilities.AbilityEquipped += onEquipAbility;
            combatant.Abilities.AbilityUnequipped += onUnequipAbility;
            combatant.Abilities.TargetsLocked += onLockedTargets;
            combatant.Abilities.ExecuteAbilityStarted += onUseAbility;
        }

        private void unsubscribeToAbilities(Combatant combatant)
        {
            combatant.Abilities.AbilityEquipped -= onEquipAbility;
            combatant.Abilities.AbilityUnequipped -= onUnequipAbility;
            combatant.Abilities.TargetsLocked -= onLockedTargets;
            combatant.Abilities.ExecuteAbilityStarted -= onUseAbility;
        }
        #endregion


        #region Ability Responses
        private void onEquipAbility(Ability ability)
        {
            _overrideActionPrompt = true;
            _actionText = $"Left Click a Tile to Lock Targets";
        }

        private void onUnequipAbility()
        {
            _overrideActionPrompt = false;
        }

        private void onLockedTargets(Ability ability)
        {
            _overrideActionPrompt = true;
            _actionText = $"Press Enter to Use {ability.name}";
        }

        private void onUseAbility(Ability ability)
        {
            _overrideActionPrompt = true;
            _actionText = $"Using {ability.name}";
        }
        #endregion
    }
}
