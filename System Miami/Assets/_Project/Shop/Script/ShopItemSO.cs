using UnityEngine;

namespace SystemMiami.Shop
{
    [CreateAssetMenu(fileName = "shopMenu", menuName = "Scriptable Objects/New Shop Item", order = 3)]
    public class ShopItemSO : ScriptableObject, IShopItem
    {
        public string title;
        public string description;
        public int baseCost;
        public int ID;

        public string GetTitle()
        {
            return title;
        }

        public string GetDescription()
        {
            return description;
        }

        public int GetCost()
        {
            return baseCost;
        }

        public int GetDatabaseID()
        {
            return ID;
        }
    }
}
