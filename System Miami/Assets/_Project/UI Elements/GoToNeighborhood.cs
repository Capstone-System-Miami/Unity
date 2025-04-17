using UnityEngine;
using SystemMiami.Management;

// Layla
namespace SystemMiami.ui
{
    public class GoToNeighborhood : MonoBehaviour
    {
        public bool CurrentIsBoss {
            get {
                if (GAME.MGR == null) { return false; }
                return GAME.MGR?.CurrentDungeonData.difficulty == Dungeons.DifficultyLevel.BOSS; }
        }

        public void Go()
        {
            Go(CurrentIsBoss);
        }

        public void Go(bool regenerate)
        {
            GAME.MGR.GoToNeighborhood(regenerate);
        }

        private void Update()
        {
            if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Go();
            }
        }
    }
}
