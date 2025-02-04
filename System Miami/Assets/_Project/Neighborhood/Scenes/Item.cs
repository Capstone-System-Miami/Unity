using UnityEngine;

namespace SystemMiami.wtf
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public bool isDefaultItem;
    
        public virtual void Use()
        {
            Debug.Log("Using " + itemName);
        }
    }
}
