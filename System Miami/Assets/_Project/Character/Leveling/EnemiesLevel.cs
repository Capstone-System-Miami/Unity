using System.Collections;
using System.Collections.Generic;
using SystemMiami.CombatSystem;
using UnityEngine;
using SystemMiami.Dungeons;
using UnityEngine.UIElements;

namespace SystemMiami
{
    public class EnemiesLevel : MonoBehaviour
    {
        public DifficultyLevel _difficulty;
        int playerCurrentLevel; //to Set Player Level
        [SerializeField]int enemyLevel; //Enemy Level;
        [SerializeField]int levelRange; //use to set the range between the player can enemy levels
        
        Attributes attributes;
        AttributeType attributeType; //use to get a random number for the stats

        [Header("Attributes")]
        public int str; // strength
        public int con; // constitution
        public int dex; // dexterity
        public int inte; // intelligence
        public int wis; // wisdom

        public void Initialize(DifficultyLevel difficulty, int playerLevel)
        {
            playerCurrentLevel = PlayerManager.MGR.CurrentLevel;
            SetEnemyLevel(difficulty);
        }
        
        private void SetEnemyLevel(DifficultyLevel difficulty)
        {
            _difficulty = difficulty;

            //Check Player Level to set EnemyLevel
            switch (_difficulty)
            {
                case DifficultyLevel.EASY: // Dungeon Diffiulty & New GDD makes it looks like it's linear if not I will fix
                    levelRange = Random.Range(3, 5);
                    enemyLevel = playerCurrentLevel - levelRange; // if player levels is 10 then enemies level should be (5 to 7) like in doc
                    break;
                case DifficultyLevel.MEDIUM:
                    levelRange = Random.Range(0, 2);
                    enemyLevel = playerCurrentLevel - levelRange; // if player levels is 10 then enemies level should be (8 to 10) like in doc
                    break;
                case DifficultyLevel.HARD:
                    levelRange = Random.Range(0, 3);
                    enemyLevel = playerCurrentLevel + levelRange; // if player levels is 10 then enemies level should be (10 to 13) like in doc
                    break;
            }
            if (enemyLevel <= 0) // if Range goes out of bounds like 0 or -1
            {
                enemyLevel = 1;
            }

            

            // if (!TryGetComponent(out Attributes attributesComponent))
            // {
            //     Debug.LogWarning($"No Enemy Combatant found on {this}", this);
            // }
            attributes = GetComponent<Attributes>();

            // TODO: once there is a public method to add attribute sets
            // to the attributes component, use it here
            attributes.AddAttributeSet(GenerateAttributes());

            Debug.Log("Enemy Level : " + enemyLevel);
        }

        private AttributeSet GenerateAttributes() // Set enemy stat using leveling System
        {
            AttributeSet additionalAttributes = new();
            int[] stats = new int[5];
            for (int i = 0; i <= enemyLevel; i++) // does it for each level until it hits max
            {
                attributeType = (AttributeType)Random.Range(0, 6);
                print("statSelector" + attributeType);
                switch (attributeType)
                {
                    case AttributeType.STRENGTH:
                        stats[0]++;
                        additionalAttributes.Set(AttributeType.STRENGTH, stats[0]); 
                        print("strength level: " + str);
                        break;
                    case AttributeType.CONSTITUTION:
                        stats[1]++;
                        additionalAttributes.Set(AttributeType.CONSTITUTION, stats[1]);
                        print("Constitution level: " + con);
                        break;
                    case AttributeType.DEXTERITY:
                        stats[2]++;
                        additionalAttributes.Set(AttributeType.DEXTERITY, stats[2]);
                        print("Dexterity level: " + dex);
                        break;
                    case AttributeType.INTELLIGENCE:
                        stats[3]++;
                        additionalAttributes.Set(AttributeType.INTELLIGENCE, stats[3]);
                        print("Intelligence level: " + inte);
                        break;
                    case AttributeType.WISDOM:
                        stats[4]++;
                        additionalAttributes.Set(AttributeType.WISDOM, stats[4]);
                        print("Wisdom level: " + wis);
                        break;
                }
                
            }
            Debug.Log($"Stats {stats[0]} {stats[1]} {stats[2]} {stats[3]} {stats[4]}");
            return additionalAttributes;
        }
    }
}
