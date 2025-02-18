using UnityEngine;

namespace SystemMiami
{
    public interface IShopItem
    {
        string GetTitle();
        string GetDescription();
        int GetCost();
    }
}
