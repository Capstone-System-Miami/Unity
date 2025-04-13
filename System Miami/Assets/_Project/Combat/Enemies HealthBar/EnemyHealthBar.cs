// Johnny Sosa
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

        public void SetMaxHealth(float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
       
            fill.color = gradient.Evaluate(1f); // In theory should change color depending on the % of the slider
        }

        public void SetHealth(float currentHealth)
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        private void Start()
        {
            _combatant = GetComponent<Combatant>();
            if (_combatant != null)
            {
                SetMaxHealth(_combatant.GetCurrentHealth());
            }
        }

        void OnEnable()
        {
            GAME.MGR.damageTaken += OnDamageTaken; // DAMGAE
           
        }

        void OnDisable()
        {
            GAME.MGR.damageTaken -= OnDamageTaken; // UPDATES HEALTH BAR
        }

        void OnDamageTaken(Combatant combatant)
        {
            if (combatant == _combatant)
            {
                SetHealth(combatant.GetCurrentHealth());
            }
            Camera.main.GetComponent<CameraShake>()?.Shake(); // Added this to allow camera shake
        }
 
    }
}

