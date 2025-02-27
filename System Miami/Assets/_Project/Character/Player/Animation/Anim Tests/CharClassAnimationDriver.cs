using System.Collections.Generic;
using SystemMiami.Animation;
using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.Drivers
{
    public class CharClassAnimationDriver : MonoBehaviour
    {
        [SerializeField] private CharacterClassType characterClass;
        [SerializeField] private bool usePreExistingClass;
        [SerializeField] private bool assignOnStart;

        [SerializeField] private PlayerCombatant player;
        [SerializeField] private Attributes playerAttributes;


        [Header("Internal Refs")]
        [SerializeField] private StandardAnimSetSO fighterAnimSet;
        [SerializeField] private StandardAnimSetSO mageAnimSet;
        [SerializeField] private StandardAnimSetSO rogueAnimSet;
        [SerializeField] private StandardAnimSetSO tankAnimSet;
        [SerializeField] private StandardAnimSetSO noClassAnimSet;

        private void Awake()
        {
            if (assignOnStart)
            {
                SetPlayerStandardAnims();
            }
        }

        public void SetPlayerStandardAnims()
        {
            StandardAnimSet playerAnimSet;

            if (usePreExistingClass)
            {
                playerAnimSet = playerAttributes._characterClass switch
                {
                    CharacterClassType.FIGHTER  => fighterAnimSet.CreateSet(),
                    CharacterClassType.ROGUE    => rogueAnimSet.CreateSet(),
                    CharacterClassType.MAGE     => mageAnimSet.CreateSet(),
                    CharacterClassType.TANK     => tankAnimSet.CreateSet(),
                    _                           => noClassAnimSet.CreateSet()
                };
            }
            else
            {
                playerAnimSet = characterClass switch
                {
                    CharacterClassType.FIGHTER => fighterAnimSet.CreateSet(),
                    CharacterClassType.ROGUE => rogueAnimSet.CreateSet(),
                    CharacterClassType.MAGE => mageAnimSet.CreateSet(),
                    CharacterClassType.TANK => tankAnimSet.CreateSet(),
                    _ => noClassAnimSet.CreateSet()
                };
            }

            player.SetAnimSet(playerAnimSet);
        }
    }
}
