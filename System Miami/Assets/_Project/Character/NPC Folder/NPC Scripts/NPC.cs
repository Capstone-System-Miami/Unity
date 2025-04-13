using SystemMiami.Shop;
using UnityEngine;
using SystemMiami.Management;
using SystemMiami.ui;
using Random = UnityEngine.Random;

namespace SystemMiami
{
    public enum NPCType
    {
        QuestGiver,
        ShopKeeper,
        Dialogue
    };

    public class NPC : MonoBehaviour
    {
        public string npcName;
        public SpriteRenderer sprite;

        public NPCInfoSO npcInfoSo;

        public bool assigned;

        public GameObject questPanel;
        public GameObject shopPanel;
        public string[] DialogueStrings;

        public NPCType myType;

        QuestGiver questGiver;
        ShopKeeper shopKeeper;

        public void Initialize(NPCType npcType)
        {
            myType = npcType;
            npcName = npcInfoSo.possibleNames[Random.Range(0, npcInfoSo.possibleNames.Count)];
            sprite.sprite = npcInfoSo.possibleSprites[Random.Range(0, npcInfoSo.possibleSprites.Count)];
            gameObject.name = npcName;
            if (myType == NPCType.QuestGiver)
            {
                questGiver = gameObject.AddComponent<QuestGiver>();
                questGiver.Initialize(npcInfoSo, npcName, questPanel);
            }
            else if (myType == NPCType.ShopKeeper)
            {
                shopKeeper = gameObject.AddComponent<ShopKeeper>();
                shopKeeper.Initialize(npcInfoSo, npcName, shopPanel);
            }
            else if (DialogueStrings.Length == 0)
            {
                DialogueStrings = new string[]
                {
                    "Placeholder NPC Dialogue One,",
                    "Placeholder NPC Dialogue Two.",
                };
            }
        }

        public void InteractWithPlayer()
        {
            if (myType == NPCType.QuestGiver)
            {
                questGiver.TalkToQuestGiver();
            }
            else if (myType == NPCType.ShopKeeper)
            {
                shopKeeper.TalkToShopKeeper();
            }
            else
            {
                UI.MGR.StartDialogue(
                    this,
                    false,
                    false,
                    true,
                    npcName,
                    DialogueStrings);
            }
        }
    }
}
