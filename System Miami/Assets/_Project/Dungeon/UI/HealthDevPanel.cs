using SystemMiami.CombatSystem;
using SystemMiami.Management;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class HealthDevPanel : MonoBehaviour
    {
        [SerializeField] private Combatant _combatant;

        [SerializeField] private Image _background;
        [SerializeField] private TextBox _nameBox;
        [SerializeField] private LabeledField _health;

        private Vector3 _currentPosition;

        private void OnEnable()
        {
            GAME.MGR.CombatantDeath += onCombatantDeath;
        }

        private void OnDisable()
        {
            GAME.MGR.CombatantDeath -= onCombatantDeath;
        }

        private void Start()
        {
            Hide();
        }

        private void Update()
        {
            if (_combatant == null) return;

            updateHealth();
        }

        private void updateHealth()
        {
            string result = $"{_combatant.Health.Get()} / {_combatant.Health.GetMax()}";
            _health.Value.SetForeground(result);
        }

        private void onCombatantDeath(Combatant deadCombatant)
        {
            if (_combatant == deadCombatant)
            {
                SetDead();
            }
        }

        public void SetCombatant(Combatant enemy, int index)
        {
            Show();
            _combatant = enemy;
            _nameBox.SetForeground($"{enemy.name}");
            _health.Label.SetForeground("Health:");
        }

        public void Show()
        {
            _health.enabled = true;
            _background.enabled = true;
            _nameBox.ShowBackground();
            _nameBox.ShowForeground();
        }

        public void Hide()
        {
            _health.enabled = false;
            _background.enabled = false;
            _nameBox.HideBackground();
            _nameBox.HideForeground();
        }

        public void SetDead()
        {
            _combatant = null;

            _health.SetBackground(Color.grey);

            _health.Label.SetBackground(Color.grey);
            _health.Label.SetForeground("dead");
            _health.Label.SetForeground(Color.white);

            _health.Value.SetBackground(Color.grey);
            _health.Value.SetForeground("X");
            _health.Value.SetForeground(Color.white);
        }
    }
}
