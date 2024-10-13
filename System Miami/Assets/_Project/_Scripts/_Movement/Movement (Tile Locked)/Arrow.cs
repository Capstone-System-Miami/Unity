using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SystemMiami
{
    public class Arrow : MonoBehaviour
    {

        public enum ArrowDirection
        {
            None = 0,
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            TopLeft = 5,
            BottomLeft = 6,
            TopRight = 7,
            BottomRight = 8,
            UpFinished = 9,
            DownFinished = 10,
            LeftFinished = 11,
            RightFinished = 12
        }

        [Header("Locations")]
        [SerializeField] private OverlayTile currentTile;
        [SerializeField] private OverlayTile previousTile;
        [SerializeField] private OverlayTile futureTile;

        [Header("Visual")]
        [SerializeField] private Sprite[] arrows;
        [SerializeField] private int arrowNumber;

        [Header("Components")]
        [SerializeField] private SpriteRenderer _sprite;

        private void Awake()
        {
            if(_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        }

        public void CalculateArrow(){
            bool isFinal = futureTile == null;

            Vector2Int pastDirection = previousTile != null ? (Vector2Int)(currentTile.gridLocation - previousTile.gridLocation) : new Vector2Int(0, 0);
            Vector2Int futureDirection = futureTile != null ? (Vector2Int)(futureTile.gridLocation - currentTile.gridLocation) : new Vector2Int(0, 0);
            Vector2Int direction = pastDirection != futureDirection ? pastDirection + futureDirection : futureDirection;

            arrowNumber = (int)ArrowDirection.None;

            if (direction == new Vector2(0, 1) && !isFinal)
            {
                arrowNumber = (int)ArrowDirection.Up;
            }

            if (direction == new Vector2(0, -1) && !isFinal)
            {
                arrowNumber = (int)ArrowDirection.Down;
            }

            if (direction == new Vector2(1, 0) && !isFinal)
            {
                arrowNumber = (int)ArrowDirection.Right;
            }

            if (direction == new Vector2(-1, 0) && !isFinal)
            {
                arrowNumber = (int)ArrowDirection.Left;
            }

            if (direction == new Vector2(1, 1))
            {
                if(pastDirection.y < futureDirection.y)
                    arrowNumber = (int)ArrowDirection.BottomLeft;
                else
                    arrowNumber = (int)ArrowDirection.TopRight;
            }

            if (direction == new Vector2(-1, 1))
            {
                if (pastDirection.y < futureDirection.y)
                    arrowNumber = (int)(int)ArrowDirection.BottomRight;
                else
                    arrowNumber = (int)ArrowDirection.TopLeft;
            }

            if (direction == new Vector2(1, -1))
            {
                if (pastDirection.y > futureDirection.y)
                    arrowNumber = (int)ArrowDirection.TopLeft;
                else
                    arrowNumber = (int)ArrowDirection.BottomRight;
            }

            if (direction == new Vector2(-1, -1))
            {
                if (pastDirection.y > futureDirection.y)
                    arrowNumber = (int)ArrowDirection.TopRight;
                else
                    arrowNumber = (int)ArrowDirection.BottomLeft;
            }

            if (direction == new Vector2(0, 1) && isFinal)
            {
                arrowNumber = (int)ArrowDirection.UpFinished;
            }

            if (direction == new Vector2(0, -1) && isFinal)
            {
                arrowNumber = (int)ArrowDirection.DownFinished;
            }

            if (direction == new Vector2(-1, 0) && isFinal)
            {
                arrowNumber = (int)ArrowDirection.LeftFinished;
            }

            if (direction == new Vector2(1, 0) && isFinal)
            {
                arrowNumber = (int)ArrowDirection.RightFinished;
            }


            _sprite.sprite = arrows[arrowNumber];

        }
        public void SetTileData(OverlayTile currentTile, OverlayTile previousTile, OverlayTile futureTile)
        {
            this.currentTile = currentTile;
            this.previousTile = previousTile;
            this.futureTile = futureTile;

            CalculateArrow();
        }


    }

}
