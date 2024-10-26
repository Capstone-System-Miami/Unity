// Author: Layla Hoey
using UnityEngine;
using SystemMiami.AbilitySystem;

namespace SystemMiami.UI
{
    public class AbilitySlot : MonoBehaviour
    {
        public SpriteBox Background;

        public TextBox Number;
        public SpriteBox NumberBKG;

        public TextBox Name;
        public SpriteBox NameBKG;

        public SpriteBox Icon;
        public SpriteBox IconBKG;

        private int _index;
        private AbilityType _type;

        public void Initialize(int index, AbilityType type)
        {
            _index = index;
            _type = type;

            Number.Set(_index.ToString("D2"));

            Empty();
        }

        public void Fill(Ability ability)
        {
            Name.Set(ability.name);
            Icon.Set(ability.Icon);

            // TODO:
            // Set colors according to type
        }

        public void Empty()
        {
            // TODO:
            // Set state to empty (colors, text, etc)
        }

        public void Highlight()
        {
            // TODO
        }

        public void UnHighlight()
        {
            // TODO

        }

        public void Click()
        {
            // TODO
        }
    }
}
