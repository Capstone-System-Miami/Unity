using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class ShopImage : MonoBehaviour
    {
       public Image image;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}
