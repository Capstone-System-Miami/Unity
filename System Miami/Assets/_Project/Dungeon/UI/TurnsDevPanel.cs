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
        #endregion

        #region Properties
        private Combatant turnOwner
        {
            get
            {
                return TurnManager.MGR.CurrentTurnOwner;
            }
        }
        #endregion

        #region Bools
        private bool _promptsEnabled;
        private bool _overrideActionPrompt;
        #endregion

        public void SetMovementPrompt(string prompt)
        {
            _promptToDisplay = prompt;
        }

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
            if (turnOwner == null) { return; }

            updateCombatantPanel();
            updatePhasePanel();
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
            Phase currentPhase = turnOwner == null ? Phase.None : turnOwner.CurrentPhase;

            Color phaseColor = currentPhase switch
            {
                Phase.Movement => _movementColor,
                Phase.Action => _actionColor,
                Phase.None => Color.black,
                _ => Color.black
            };

            _phaseField.Value.SetForeground($"{turnOwner.CurrentPhase}");
            _phaseField.Value.SetForeground(phaseColor);
        }
        #endregion
    }
}
