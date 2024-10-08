using UnityEngine;

namespace SystemMiami.Abilities
{
    // TODO
    // This is wholely incomplete.
    // Depends on the creation & refactoring of other scripts.
    public abstract class Ability : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private AbilityType _type;

        private ResourceType _cost;

        public abstract void Preview();
        public abstract void Use();
    }
}
