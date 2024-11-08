using UnityEngine;

namespace SystemMiami.CombatSystem
{
    public class PlayerController : CombatantController
    {
        [SerializeField] private KeyCode _endTurnKey;
        [SerializeField] private KeyCode _endMovementPhaseKey;

        private OverlayTile _mostRecentMouseTile;


    }
}
