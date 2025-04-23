using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [CreateAssetMenu(fileName = "PlayerSprites", menuName = "PlayerSprites")]
    public class PlayerSprites : ScriptableObject
    {
        public Sprite tankOutroSprite;
        public Sprite mageOutroSprite;
        public Sprite rogueOutroSprite;
        public Sprite fighterOutroSprite;
        
        public Sprite tankSprite;
        public Sprite mageSprite;
        public Sprite rogueSprite;
        public Sprite fighterSprite;

        public Sprite GetClassPFP(CharacterClassType charClass)
        {
            return charClass switch {
                CharacterClassType.TANK     => tankSprite,
                CharacterClassType.MAGE     => mageSprite,
                CharacterClassType.ROGUE    => rogueSprite,
                CharacterClassType.FIGHTER  => fighterSprite,
                _                           => tankSprite,
            };
        }
    }
}
