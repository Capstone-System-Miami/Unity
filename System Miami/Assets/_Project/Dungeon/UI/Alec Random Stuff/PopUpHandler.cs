using System.Collections;
using System.Collections.Generic;
using SystemMiami.AbilitySystem;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;
using SystemMiami.CombatRefactor;
namespace SystemMiami
{
    public class PopUpHandler : MonoBehaviour
    {
        public static PopUpHandler Instance;
        public RectTransform Popup;
        public ItemData AssignedCombatAction;
        public Text DescriptionText;
        public Vector2 offset;
        public Text ItemName;

       // private bool OnShown => Popup.gameObject.activeSelf;

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
            if (Popup.gameObject.activeSelf)
            {
                SetPopupPosition();
            }
        }

        public void SetPopupAblility(ItemData itemData)
        {
            
                AssignedCombatAction = itemData;
                Popup.gameObject.SetActive(true);
                BindText();
            

        }


        public void SetPopupAblility()
        {
            
            Popup.gameObject.SetActive(false);

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
            DescriptionText.text = AssignedCombatAction.Description;
            ItemName.text = AssignedCombatAction.Name;
        }

    }
}
