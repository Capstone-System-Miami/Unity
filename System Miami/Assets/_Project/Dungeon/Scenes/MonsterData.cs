using UnityEngine;

[CreateAssetMenu(fileName = "NewMonster", menuName = "Monster/Create New Monster")]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public Sprite monsterSprite;
}
