using UnityEngine;
using SystemMiami.Management;

namespace SystemMiami.ui
{
    public class GoToNeighborhood : MonoBehaviour
    {
        public void Go(bool regenerate)
        {
            GAME.MGR.GoToNeighborhood(regenerate);
        }
    }
}
