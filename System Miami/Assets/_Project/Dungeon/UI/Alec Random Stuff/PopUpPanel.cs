using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class PopUpPanel : MonoBehaviour
    {
        public GameObject popupPanel; // Assign your panel in the inspector

        void Start()
        {
            if (popupPanel != null)
            {
                popupPanel.SetActive(false); // Start with the panel off
            }
        }

        public void TogglePopup()
        {
            if (popupPanel != null)
            {
                popupPanel.SetActive(!popupPanel.activeSelf); // Toggle panel on and off
            }
        }
    }
}
