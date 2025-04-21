// Author: Johnny Sosa

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SystemMiami
{
    public class SlideShowIntro : MonoBehaviour
    {

        public Sprite[] slides;
        public string[] slideTexts;
        public Image slideshowImages;
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI slideText;
        public Button continueButton;
        public float fadeDuration = 1f;


        private int currentSlide = 0;
        private bool waitingForInput = false;




        // Start is called before the first frame update
        void Start()
        {
            continueButton.onClick.AddListener(AdvanceSlide);
            continueButton.interactable = false;
            StartCoroutine(PlaySlideShow());
        
        }

        IEnumerator PlaySlideShow()
        {
            while (currentSlide < slides.Length)
            {
                slideshowImages.sprite = slides[currentSlide]; // Chnages pictures
                slideText.text = slideTexts[currentSlide]; // Changes the text

                yield return StartCoroutine(Fade(0f, 1f)); // Start Fade

                waitingForInput = true;
                continueButton.interactable = true;

                while (waitingForInput)
                {
                    yield return null;
                }

                continueButton.interactable = false;
                yield return StartCoroutine(Fade(1f, 0f)); // End fade

                currentSlide++;

            }

            SceneManager.LoadScene("Menu Scene");
            Debug.Log("SlideShow Finished");
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
                float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;

            }
            canvasGroup.alpha = to;

        }

    }
}
