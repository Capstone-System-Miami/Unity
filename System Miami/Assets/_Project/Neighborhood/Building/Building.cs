using FunkyCode;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private List<Tilemap> tilemapsToToggleTransparent;
        [SerializeField] private List<Light2D> lightsToToggle;

        public void TurnTransparent()
        {
            //tilemapsToToggleTransparent.ForEach(tilemap => tilemap.)
        }

        public void TurnOpaque()
        {

        }
    }
}
