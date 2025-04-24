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
        public CustomString[] names;
        public CanvasGroup canvasGroup;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI nameText;
        public Button continueButton;
        public float startDelay = 3f;
        public float endDelay = 3f;
        public float readTime = 2f;
        public float fadeDuration = 1f;

        private int currentSlide = 0;

        void Start()
        {
            currentSlide = 0;
            canvasGroup.alpha = 0;
            StartCoroutine(PlaySlideShow());
        }

        IEnumerator PlaySlideShow()
        {
            yield return new WaitForSeconds(startDelay);

            while (currentSlide < titles.Length && currentSlide < names.Length)
            {
                titleText.text = titles[currentSlide];
                nameText.text = names[currentSlide].text;
                yield return StartCoroutine(Fade(0f, 1f)); // Start Fade
                yield return new WaitForSeconds(readTime);
                yield return StartCoroutine(Fade(1f, 0f)); // End fade
                currentSlide++;
            }

            yield return new WaitForSeconds(endDelay);

            Debug.Log("Credits Finished");
            SceneManager.LoadScene("Menu Scene"); // Takes you back to Main
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

[System.Serializable]
public class CustomString
{
    [SerializeField,TextArea] public string text;
}
