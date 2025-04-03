using System.Collections.Generic;
using SystemMiami.Animation;
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami.Drivers
{
    public class CharClassAnimationDriver : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private dbug log;

        [SerializeField] private CharacterClassType characterClass;
        [SerializeField] private bool usePreExistingClass;
        [SerializeField] private bool assignOnStart;

        [SerializeField] private TopDownMovement neighborhoodMovement;
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
            if (!TryGetComponent(out neighborhoodMovement))
            {
                log.error($"{name}'s {this} didn't find a TopDownMovement");
            }
            else
            {
                log.error($"yes did find {neighborhoodMovement}");
            }

            if (!TryGetComponent(out player))
            {
                log.error($"{name}'s {this} didn't find a PlayerCombatant");
            }
            else
            {
                log.error($"yes did find {player}");
            }

            if (!TryGetComponent(out playerAttributes))
            {
                log.error($"{name}'s {this} didn't find an Inventory");
            }
            else
            {
                log.error($"yes did find {playerAttributes}");
            }

            if (assignOnStart)
            {
                SetPlayerStandardAnims();
            }
        }

        public void SetPlayerStandardAnims()
        {
            CharacterClassType usingCaracterClass = usePreExistingClass
                ? playerAttributes._characterClass
                : characterClass;

            StandardAnimSet playerAnimSet = usingCaracterClass switch
                {
                    CharacterClassType.FIGHTER  => fighterAnimSet.CreateSet(),
                    CharacterClassType.ROGUE    => rogueAnimSet.CreateSet(),
                    CharacterClassType.MAGE     => mageAnimSet.CreateSet(),
                    CharacterClassType.TANK     => tankAnimSet.CreateSet(),
                    _                           => noClassAnimSet.CreateSet()
                };

            neighborhoodMovement.SetAnimSet(playerAnimSet);
            player.SetAnimSet(playerAnimSet);
        }
    }
}
