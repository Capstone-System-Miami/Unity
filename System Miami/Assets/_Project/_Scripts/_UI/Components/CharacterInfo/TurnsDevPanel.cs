using SystemMiami.AbilitySystem;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class TurnsDevPanel : MonoBehaviour
    {
        [SerializeField] private TextBox _panelLabel;

        [SerializeField] private LabeledField _turn;
        [SerializeField] private Color _playerColor = Color.white;
        [SerializeField] private Color _enemyColor = Color.white;

        [SerializeField] private LabeledField _phase;

        [SerializeField] TextBox _prompts;
        [SerializeField] private Color _promptsTextColor = Color.white;

        private Combatant _turnOwner;
        private Abilities _turnOwnerAbilities;

        private string _turnText;
        private string _phaseText;
        private string _promptsText;

        private bool _promptsEnabled;
        private bool _canAct;

        private void OnEnable()
        {
            TurnManager.Instance.BeginTurn += onBeginTurn;
            TurnManager.Instance.NewTurnPhase += onNewTurnPhase;
        }

        private void OnDisable()
        {
            TurnManager.Instance.BeginTurn -= onBeginTurn;
            TurnManager.Instance.NewTurnPhase -= onNewTurnPhase;
        }

        private void Start()
        {
            _panelLabel.SetForeground("Turn Indicator Dev Panel");
            _turn.Label.SetForeground("Combatant:");
            _phase.Label.SetForeground("Phase:");
        }

        private void Update()
        {
            _turn.Value.SetForeground(_turnText);
            _phase.Value.SetForeground(_phaseText);

            if (_promptsEnabled)
            {
                _prompts.SetForeground(_promptsText);
            }
        }

        private void onBeginTurn(Combatant combatant)
        {
            _turnOwner = combatant;

            if (combatant.TryGetComponent(out _turnOwnerAbilities))
            {
                _turnOwnerAbilities.EquipAbility += onEquipAbility;
                _turnOwnerAbilities.UnequipAbility += onUnequipAbility;
                _turnOwnerAbilities.LockTargets += onLockedTargets;
                _turnOwnerAbilities.UseAbility += onUseAbility;
            }
            else if (_turnOwnerAbilities != null)
            {
                _turnOwnerAbilities.EquipAbility -= onEquipAbility;
                _turnOwnerAbilities.UnequipAbility -= onUnequipAbility;
                _turnOwnerAbilities.LockTargets -= onLockedTargets;
                _turnOwnerAbilities.UseAbility -= onUseAbility;
            }

            if (combatant.Controller is CombatSystem.PlayerController)
            {
                _turnText = $"Player";
                _turn.Value.SetForeground(_playerColor);
                enablePrompts();
            }
            else
            {
                _turnText = $"Enemy {combatant.ID}";
                _turn.Value.SetForeground(_enemyColor);
                disablePrompts();
            }
        }

        private void onNewTurnPhase(Phase phase)
        {
            switch (phase)
            {
                default:
                case Phase.MovementPhase:
                    _phaseText = "Movement";
                    _promptsText = $"Click a tile to move,\n\n" +
                        $"Press E to end Movement Phase\n\n" +
                        $"Or Press Q to end Turn";
                    break;

                case Phase.ActionPhase:
                    _phaseText = "Action";
                    _promptsText = $"Select an Ability,\n\n Or Press Q to end Turn";
                    _canAct = true;
                    break;
            }
        }

        private void onEquipAbility(AbilityType type, int index)
        {
            enablePrompts();
            _promptsText = $"Left Click To Lock Targets";
        }

        private void onUnequipAbility()
        {           
            _promptsText = _canAct ? $"Select an Ability\n\nOr press Q to End Turn" : $"Press Q to End Turn";
        }

        private void onLockedTargets(Ability ability)
        {
            enablePrompts();
            _promptsText = $"Press Enter to Use {ability.name}";
        }

        private void onUseAbility(Ability ability)
        {
            enablePrompts();
            _promptsText = $"Using {ability.name}";
            _canAct = false;
        }

        private void enablePrompts()
        {
            _promptsEnabled = true;
            _prompts.ShowBackground();
            _prompts.ShowForeground();
            _prompts.SetForeground(_promptsTextColor);
        }

        private void disablePrompts()
        {
            _promptsEnabled = false;
            _prompts.HideBackground();
            _prompts.HideForeground();
        }
    }
}
