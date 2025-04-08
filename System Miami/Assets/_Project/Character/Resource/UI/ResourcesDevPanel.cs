using SystemMiami.CombatSystem;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class ResourcesDevPanel : MonoBehaviour
    {
        [SerializeField] private Combatant _player;

       /** [SerializeField] private TextBox _panelLabel; 

        [SerializeField] private LabeledField _health;
        [SerializeField] private LabeledField _stamina;
        [SerializeField] private LabeledField _mana;
        [SerializeField] private LabeledField _speed;*/
        [SerializeField] private Image _healthImage;
        [SerializeField] private Image _staminaImage;
        [SerializeField] private Image _manaImage;
        [SerializeField] private Image _speedImage;
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
           /* _panelLabel.SetForeground("Resources Dev Panel");

            _health.Label.SetForeground("Health:");
            _stamina.Label.SetForeground("Stamina:");
            _mana.Label.SetForeground("Mana:");
            _speed.Label.SetForeground("Speed:");*/
        }

        private void updateHealth()
        {
           if (_player.Health == null) { return; } /*

            string result = $"{_player.Health.Get()} / {_player.Health.GetMax()}";

            _health.Value.SetForeground(result);         */

            _healthImage.fillAmount = _player.Health.Get() / _player.Health.GetMax();
        }

        private void updateStamina()
        {
            if (_player.Stamina == null) { return; } /*

            string result = $"{_player.Stamina.Get()} / {_player.Stamina.GetMax()}";

            _stamina.Value.SetForeground(result); */

            _staminaImage.fillAmount = _player.Stamina.Get() / _player.Stamina.GetMax(); 
        }

        private void updateMana()
        {
            if (_player.Mana == null) { return; } /*

            string result = $"{_player.Mana.Get()} / {_player.Mana.GetMax()}";

            _mana.Value.SetForeground(result); */

            _manaImage.fillAmount = _player.Mana.Get() / _player.Mana.GetMax();
        }

        private void updateSpeed()
        {
            if (_player.Speed == null) { return; } /*

            string result = $"{_player.Speed.Get()} / {_player.Speed.GetMax()}";

            _speed.Value.SetForeground(result); */

            _speedImage.fillAmount = _player.Speed.Get() / _player.Speed.GetMax();
        }
    }
}
