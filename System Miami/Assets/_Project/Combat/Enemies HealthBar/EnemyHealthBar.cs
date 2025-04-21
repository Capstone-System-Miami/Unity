// Author: Johnny Sosa
using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using SystemMiami.CombatRefactor;
using SystemMiami.InventorySystem;
using SystemMiami.Management;
using SystemMiami.Utilities;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.CombatSystem;


namespace SystemMiami
{
    public class EnemyHealthBar : MonoBehaviour
    {
        public Slider slider;
        public Gradient gradient;
        public Image fill;
        private Combatant _combatant;

        private void Awake()
        {
            _combatant = GetComponent<Combatant>();
        }

        void OnEnable()
        {
            GAME.MGR.damageTaken += OnDamageTaken; // DAMGAE
        }

        void OnDisable()
        {
            GAME.MGR.damageTaken -= OnDamageTaken; // UPDATES HEALTH BAR
        }

        private void Start()
        {
            StartCoroutine(SetOnceInitialized());
        }

        public void SetMaxHealth(float maxHealth)
        {
            // NOTE: (layla) Removed the line setting the combatants current health.
            slider.maxValue = maxHealth;
            fill.color = gradient.Evaluate(1f); // In theory should change color depending on the % of the slider
        }

        public void SetHealth(float currentHealth)
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        public void Reset()
        {
            SetMaxHealth(_combatant.Health.GetMax());
            SetHealth(_combatant.Health.GetMax());
        }

        public void Recalc()
        {
            SetMaxHealth(_combatant.Health.GetMax());
            SetHealth(_combatant.Health.Get());
        }

        private IEnumerator SetOnceInitialized()
        {
            yield return new WaitUntil( () => _combatant != null & _combatant.Initialized);
            Reset();
        }

        private void OnDamageTaken(Combatant combatant)
        {
            if (combatant == _combatant)
            {
                // Status effects will change the health of the enemy,
                // so we want to make sure their max is set frequently.
                SetMaxHealth(combatant.Health.GetMax());
                SetHealth(combatant.Health.Get());
            }
            Camera.main.GetComponent<CameraShake>()?.Shake(); // Added this to allow camera shake
        }
 
    }
}

