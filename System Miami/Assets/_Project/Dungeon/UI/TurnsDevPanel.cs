// Authors: Layla
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
        #endregion

        #region Private
        private string _promptToDisplay;
        private bool _promptsEnabled;
        private bool _overrideActionPrompt;
        #endregion

        #region Properties
        private Combatant turnOwner => TurnManager.MGR?.CurrentTurnOwner;

        private string turnOwnerName => turnOwner?.name ?? "None";

        private Color turnOwnerColor => turnOwner?.ColorTag ?? Color.black;

        private Phase currentPhase => turnOwner?.CurrentPhase ?? Phase.None;

        private Color phaseColor => currentPhase switch
        {
            Phase.Movement  => _movementColor,
            Phase.Action    => _actionColor,
            Phase.None      => Color.black,
            _               => Color.black
        };

        #endregion

        public void SetMovementPrompt(string prompt)
        {
            _promptToDisplay = prompt;
        }

        // Methods
        #region Unity
        private void Start() 
        {
            _panelLabel.SetForeground("Turn Indicator");
            _combatantField.Label.SetForeground("Combatant:");
            _phaseField.Label.SetForeground("Phase:");
        }

        private void Update()
        {
            if (turnOwner == null) { return; }

            updateCombatantPanel();
            updatePhasePanel();
        }
        #endregion


        #region Panel Updates
        private void updateCombatantPanel()
        {
            _combatantField.Value.SetForeground($"{turnOwnerName}");
            _combatantField.Value.SetForeground(turnOwnerColor);
        }

        private void updatePhasePanel()
        {
            _phaseField.Value.SetForeground($"{currentPhase}");
            _phaseField.Value.SetForeground(phaseColor);
        }
        #endregion
    }
}
