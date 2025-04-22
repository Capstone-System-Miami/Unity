// Author: Johnny Sosa

using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SystemMiami.Management;
using UnityEngine;

namespace SystemMiami
{
    public class Outro : MonoBehaviour
    {
       public CanvasGroup canvasGroup;
       public float fadeDuration = 3f;
       public Image fadeImage;
       public PlayerSprites player;

       private void Start()
       {
           StartCoroutine(ShowPicThenChangeScene());
       }
       
       public IEnumerator ShowPicThenChangeScene()
       {
           fadeImage.sprite = GetPlayerClass();
           yield return Fade(0f, 1f); // Fade in
           yield return new WaitForSeconds(2f); // Wait for 2 seconds
           yield return Fade(1f, 0f); // Fade out
           GAME.MGR.GoToCredits();
       }

       Sprite GetPlayerClass()
       {
           CharacterClassType type = PlayerManager.MGR.GetComponent<Attributes>()._characterClass;
           
           Sprite sprite  = type switch
           {
               CharacterClassType.MAGE => player.mageOutroSprite,
               CharacterClassType.TANK => player.tankOutroSprite,
               CharacterClassType.ROGUE => player.rogueOutroSprite,
               CharacterClassType.FIGHTER => player.fighterOutroSprite,
               _ => player.fighterOutroSprite
           };
           return sprite;
       }
       
       
       IEnumerator Fade(float from, float to)
       {
           float timer = 0f;
           while (timer < fadeDuration)
           {
               timer += Time.deltaTime;
               float alpha = Mathf.Lerp(from, to, timer / fadeDuration); // Fade effect is handled here
               canvasGroup.alpha = alpha;
               yield return null;

           }
           canvasGroup.alpha = to;

       }
    }
}
