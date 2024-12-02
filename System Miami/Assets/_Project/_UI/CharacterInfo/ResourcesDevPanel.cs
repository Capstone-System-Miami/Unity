using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami
{
    public class ResourcesDevPanel : MonoBehaviour
    {
        [SerializeField] private Combatant _player;

        [SerializeField] private TextBox _panelLabel;

        [SerializeField] private LabeledField _health;
        [SerializeField] private LabeledField _stamina;
        [SerializeField] private LabeledField _mana;
        [SerializeField] private LabeledField _speed;

        private void Start()
        {
            if (_player == null) { return; }

            initializeLabels();
        }

        private void Update()
        {
            Combatant player = TurnManager.MGR.playerCharacter;

            if (_player != player && player != null)
            {
                _player = player;
            }

            if (_player == null) { return; }

            updateHealth();
            updateStamina();
            updateMana();
            updateSpeed();
        }

        private void initializeLabels()
        {
            _panelLabel.SetForeground("Resources Dev Panel");

            _health.Label.SetForeground("Health:");
            _stamina.Label.SetForeground("Stamina:");
            _mana.Label.SetForeground("Mana:");
            _speed.Label.SetForeground("Speed:");
        }

        private void updateHealth()
        {
            if (_health == null) { return; }

            string result = $"{_player.Health.Get()} / {_player.Health.GetMax()}";

            _health.Value.SetForeground(result);
        }

        private void updateStamina()
        {
            if (_stamina == null) { return; }

            string result = $"{_player.Stamina.Get()} / {_player.Stamina.GetMax()}";

            _stamina.Value.SetForeground(result);
        }

        private void updateMana()
        {
            if (_mana == null) { return; }

            string result = $"{_player.Mana.Get()} / {_player.Mana.GetMax()}";

            _mana.Value.SetForeground(result);
        }

        private void updateSpeed()
        {
            if (_speed == null) { return; }

            string result = $"{_player.Speed.Get()} / {_player.Speed.GetMax()}";

            _speed.Value.SetForeground(result);
        }
    }
}
