// Author: Alec
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour
    {
        // The height of the overlay tile.
        // Presumably NONE, since it's flat.
        // But it's here in case we need it later.
        public BlockHeight height;

        // ??
        public int G;
        public int H;

        // Reference to the character currently on this tile
        public Combatant currentCharacter;

        // ??
        public int F { get { return G + H; } }

        public bool isBlocked;

        public OverlayTile previous;

        public Vector3Int gridLocation;
  
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HideTile();
            }
        }

        public void ShowTile()
        {
            // Max vals means white.
            gameObject.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, 1);
        }

        public void HideTile()
        {
            // Alpha = 0 means transparent.
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
