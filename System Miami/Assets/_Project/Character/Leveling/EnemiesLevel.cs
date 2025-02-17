using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class EnemiesLevel : MonoBehaviour
    {
        public Difficulty _difficulty;
        PlayerLevel _playerLevel; //to call Player Level Script
        int playerCurrentLevel; //to Set Player Level
        int enemyLevel; //Enemy Level;
        int levelRange; //use to set the range between the player can enemy levels
        int statSelector; //use to get a random number for the stats

        [Header("Attributes")]
        public int str = 1; // strength
        public int con = 1; // constitution
        public int dex = 1; // dexterity
        public int inte = 1; // intelligence
        public int wis = 1; // wisdom

        // Start is called before the first frame update
        void Start()
        {
            _playerLevel = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLevel>(); // Look for player to read level
            playerCurrentLevel = _playerLevel.level;
            EnemyLevel();
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        void EnemyLevel()
        {
            //Check Player Level to set EnemyLevel
            switch (_difficulty)
            {
                case Difficulty.Easy: // Dungeon Diffiulty & New GDD makes it looks like it's linear if not I will fix
                    levelRange = Random.Range(3, 5);
                    enemyLevel = playerCurrentLevel - levelRange; // if player levels is 10 then enemies level should be (5 to 7) like in doc
                    break;
                case Difficulty.Medium:
                    levelRange = Random.Range(0, 2);
                    enemyLevel = playerCurrentLevel - levelRange; // if player levels is 10 then enemies level should be (8 to 10) like in doc
                    break;
                case Difficulty.Hard:
                    levelRange = Random.Range(0, 3);
                    enemyLevel = playerCurrentLevel + levelRange; // if player levels is 10 then enemies level should be (10 to 13) like in doc
                    break;
            }
            if (enemyLevel < 0) // if Range goes out of bounds like 0 or -1
            {
                enemyLevel = 1;
            }
            EnemyAttStat();
            Debug.Log("Enemy Level : " + enemyLevel);
        }
        void EnemyAttStat() // Set enemy stat using leveling System
        {
            for (int i = 0; i < enemyLevel - 1; i++) // does it for each level until it hits max
            {
                statSelector = Random.Range(1, 6);
                print("statSelector" + statSelector);
                switch (statSelector)
                {
                    case 1:
                        str++;
                        print("strength level: " + str);
                        break;
                    case 2:
                        con++;
                        print("Constitution level: " + con);
                        break;
                    case 3:
                        dex++;
                        print("Dexterity level: " + dex);
                        break;
                    case 4:
                        inte++;
                        print("Intelligence level: " + inte);
                        break;
                    case 5:
                        wis++;
                        print("Wisdom level: " + wis);
                        break;
                }
                
            }
        }
    }
    
    public enum Difficulty //Might have to be move to a dungeon manager script I have it here for now
    {
        Easy,
        Medium,
        Hard
    }
}
