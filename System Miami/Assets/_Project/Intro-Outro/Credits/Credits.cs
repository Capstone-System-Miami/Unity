using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace SystemMiami
{
    public class Credits : MonoBehaviour
    {
        public string[] titles;
        public string[] names;
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI nameText;
        public GameObject[] imageGroups;
        public Button continueButton;
        public float readTime = 2f;
        public float fadeDuration = 1f;

        private int currentSlide = 0;
        private bool waitingForInput = false;




        // Start is called before the first frame update
        void Start()
        {
            continueButton.onClick.AddListener(AdvanceSlide);
            continueButton.interactable = false;
            StartCoroutine(PlaySlideShow());


            for (int i = 0; i < names.Length; i++)
            {
                names[i] =  names[i].Replace(", ", "\n");
            }
        }

        IEnumerator PlaySlideShow()
        {
            while (currentSlide < titles.Length && currentSlide < names.Length)
            {
                titleText.text = titles[currentSlide];
                nameText.text = names[currentSlide];
                imageGroups[currentSlide].SetActive(true);
                yield return new WaitForSeconds(readTime);
                yield return StartCoroutine(Fade(0f, 1f)); // Start Fade
                yield return StartCoroutine(Fade(1f, 0f)); // End fade
                imageGroups[currentSlide].SetActive(false);
                currentSlide++;

            }
             Debug.Log("Credits Finished");
             SceneManager.LoadScene("Menu Scene"); // Takes you back to Main
        }

        public void AdvanceSlide()
        {
            waitingForInput = false; 
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
