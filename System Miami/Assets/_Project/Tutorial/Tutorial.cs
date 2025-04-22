using System;
using System.Collections;
using System.Collections.Generic;
using SystemMiami;
using SystemMiami.Management;
using SystemMiami.ui;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SystemMiami
{
    public class Tutorial : MonoBehaviour
    {
       public TutorialSlideData[] tutorialSlides;
       public DialogueWindow dialogueWindow;
       public Image dialogueImage;
       public Button continueButton;
       [SerializeField] int currentSection = -1;
       [SerializeField] int currentSlide = 0;

       public void Start()
       {
           Init();
       }
       public void Update()
       {
           if (dialogueWindow.AtLastIndex)
           {
               continueButton.gameObject.SetActive(true);
           }
           else
           {
               continueButton.gameObject.SetActive(false);
           }
           dialogueImage.sprite = tutorialSlides[currentSection].tutorialSprite[currentSlide];

          
       }
       
       public void Init()
       {
           UI.MGR.StartDialogue(this,false,false,false,tutorialSlides[0].header,tutorialSlides[0].tutorialText);
           dialogueWindow.rt.anchoredPosition = tutorialSlides[0].dialoguePositions.anchoredPosition;
           dialogueImage.sprite = tutorialSlides[0].tutorialSprite[0];
       }

       public void GoToNextSection()
       {
           if (currentSection == tutorialSlides.Length - 1) 
           {
               continueButton.onClick.RemoveAllListeners();
               continueButton.onClick.AddListener(() =>
               {
                   GAME.MGR.GoToCharacterSelect();
               });
               return;
           }
           currentSection++;
           dialogueWindow.CloseWindow();
           UI.MGR.StartDialogue(this,false,false,false,tutorialSlides[currentSection].header,tutorialSlides[currentSection].tutorialText);
           dialogueWindow.rt.anchoredPosition = tutorialSlides[currentSection].dialoguePositions.anchoredPosition;
           currentSlide = 0;
           
          
       }
       
       public void GoToNextSlide()
       {
           currentSlide++;
       }
       
    }
}

[System.Serializable]
public class TutorialSlideData
{
    public string header;
    public RectTransform dialoguePositions;
    public Sprite[] tutorialSprite;
    public string[] tutorialText;

}