// Author: Johnny
using System.Collections;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class ManaStaminaManager : MonoBehaviour
    {
        [SerializeField] private float manaRegenAmount = 10f;
        [SerializeField] private float staminaRegenAmount = 10f;
        [SerializeField] private float regenInterval = 30f;

        private Combatant _combatant;

        private void Awake()
        {
            // Get the Combatant component on the same GameObject
            _combatant = GetComponent<Combatant>();
        }

        private void OnEnable()
        {
            if (_combatant != null)
            {
                StartCoroutine(RegenerateResources());
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator RegenerateResources()
        {
            while (true)
            {
                yield return new WaitForSeconds(regenInterval);

                if (_combatant == null) yield break;

                // Regenerate Mana
                if (_combatant.Mana != null)
                {
                    _combatant.Mana.Gain(manaRegenAmount);
                    Debug.Log($"{_combatant.name} regenerated {manaRegenAmount} mana.");
                }

                // Regenerate Stamina
                if (_combatant.Stamina != null)
                {
                    _combatant.Stamina.Gain(staminaRegenAmount);
                    Debug.Log($"{_combatant.name} regenerated {staminaRegenAmount} stamina.");
                }
            }
        }
    }
}