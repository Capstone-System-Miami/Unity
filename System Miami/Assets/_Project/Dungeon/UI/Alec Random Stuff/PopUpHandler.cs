using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class PopUpHandler : MonoBehaviour
    {
        public static PopUpHandler Instance;
        public RectTransform Popup;
        public Ability ActualAbility;
        public Text DescriptionText;
        public Vector2 offset;
        public Text ItemName;

        private bool OnShown => ActualAbility != null;

        private void Awake()
        {
           if (Instance != null)
            {
                 
                Debug.LogError("There is more than one popup");
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (OnShown)
            {
                SetPopupPosition();
            }
        }

        public void SetPopupAblility(Ability ability)
        {
            ActualAbility = ability;
            Popup.gameObject.SetActive(OnShown);
            BindText();

        }


        public void SetPopupAblility()
        {
            SetPopupAblility(null);

        }

        private void SetPopupPosition()
        {
             Vector2 Mouseposition = Input.mousePosition;
             Popup.position = Mouseposition + offset + new Vector2(Popup.rect.width, Popup.rect.height) / 2f;
             
        }

        private void Start()
        {
            Popup.SetAsLastSibling();
            Popup.gameObject.SetActive (false);
            
        }

        private void BindText()
        {
            if (ActualAbility == null)
            {
                DescriptionText.text = string.Empty;
                ItemName.text = string.Empty;
                return;
            }

            DescriptionText.text = ActualAbility.Description;
            ItemName.text = ActualAbility.name;
        }

    }
}
