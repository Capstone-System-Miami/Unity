using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class OverlayTile : MonoBehaviour
    {

        public int G;
        public int H;

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
            gameObject.GetComponent<SpriteRenderer>().color = new Color (1,1,1,1);
        }
        public void HideTile()
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        }
    }
}
