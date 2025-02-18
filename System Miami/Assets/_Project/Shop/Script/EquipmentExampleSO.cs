using UnityEngine;

namespace SystemMiami.Shop
{
    [CreateAssetMenu(fileName = "New Example Equipment Item", menuName = "Scriptable Objects/Equipment Example", order = 3)]
    public class EquipmentExampleSO : ScriptableObject, IShopItem
    {
        public string title;
        public string description;
        public int baseCost;

        public int additionalCost;

        public int exampleVar;
        public double exampleVar2;


        public void DoSomething()
        {
            // Do something
        }

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
            return baseCost + additionalCost;
        }
    }
}
