using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public enum EnemyType
    {
        Regular,
        Boss
    }
    public class Enemy : Combatant
    {
        public EnemyType enemyType;
        public List<Ability> abilities = new List<Ability>();

        protected override void Start()
        {
            base.Start();
            // Initialize abilities
            // Initialize abilities
            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities[i] != null)
                {
                    // Instantiate a new instance of the ability to avoid modifying the original asset
                    Ability abilityInstance = Instantiate(abilities[i]);
                    abilityInstance.Init(this);
                    abilities[i] = abilityInstance;
                }
            }
        }

        public override void Die()
        {
            TurnManager.Instance.enemyCharacters.Remove(this);
            Destroy(gameObject);
        }
    }
}

