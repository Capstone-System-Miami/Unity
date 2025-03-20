using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public class CharacterMenuSubPanel : MonoBehaviour, ISingleSelectable
    {
        private Image background;

        int ISingleSelectable.SelectionIndex { get; set; }

        public bool IsSelected { get; private set; }

        private void Awake()
        {
            background = GetComponent<Image>();
        }

        private void Update()
        {
            background.enabled = IsSelected;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(IsSelected);
            }
        }

        public void Select()
        {
            IsSelected = true;
        }

        public void Deselect()
        {
            IsSelected = false;
        }
    }
}
