// Author: Alec
using SystemMiami.CombatSystem;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour, ITargetable
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

        public bool Valid
        {
            get
            {
                return (currentCharacter == null) && (!isBlocked);
            }
        }

        private SpriteRenderer _renderer;
        private Color _defaultColor;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _defaultColor = _renderer.color;
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HideTile();
            }
        }

        public void ShowTile()
        {
            //Debug.Log($"{name} at {gridLocation} is trying to show self");
            // Max vals means white.
            gameObject.GetComponent<SpriteRenderer>().color = new Color (1, 1, 1, 1);
        }

        public void HideTile()
        {
            // Alpha = 0 means transparent.
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }

        public void Target()
        {
            ShowTile();
        }

        public void Highlight(Color color)
        {
            ShowTile();

            _renderer.color = color;
        }

        public void UnHighlight()
        {
            HideTile();
            _renderer.color = _defaultColor;
        }

        public GameObject GameObject()
        {
            return gameObject;
        }
    }
}
