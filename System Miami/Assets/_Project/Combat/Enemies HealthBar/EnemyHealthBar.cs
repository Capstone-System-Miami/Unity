
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

        public void SetMaxHealth(float maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;

            fill.color = gradient.Evaluate(1f);
        }

        public void SetHealth(float currentHealth)
        {
            slider.value = currentHealth;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        void OnEnable()
        {
            GAME.MGR.damageTaken += OnDamageTaken;
        }

        void OnDisable()
        {
            GAME.MGR.damageTaken -= OnDamageTaken;
        }

        void OnDamageTaken(Combatant combatant)
        {
            SetHealth(combatant.GetCurrentHealth());

            Camera.main.GetComponent<CameraShake>()?.Shake(); // Added this to allow camera shake
        }
 
    }
}

