using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monsters/Monster Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public Sprite monsterSprite; // Preview image
    public GameObject monsterPrefab;
}
