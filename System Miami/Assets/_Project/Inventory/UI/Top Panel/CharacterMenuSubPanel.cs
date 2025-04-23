using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami.ui
{
    public class CharacterMenuSubPanel : MonoBehaviour, ISingleSelectable
    {
        [Header("Player Info")]
        [SerializeField] private Image characterImage;
        [SerializeField, ReadOnly] Sprite characterSprite;

        private Image background;

        int ISingleSelectable.SelectionIndex { get; set; }

        public bool IsSelected { get; private set; }

        protected virtual void Awake()
        {
            background = GetComponent<Image>();
        }

        protected virtual void Update()
        {
            background.enabled = IsSelected;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(IsSelected);
            }
        }

        public virtual void Select()
        {
            IsSelected = true;
            if (characterImage != null)
            {
                characterSprite = PlayerManager.MGR?.PlayerSprite;
                characterImage.sprite = characterSprite;
            }
        }

        public virtual void Deselect()
        {
            IsSelected = false;
        }
    }
}
