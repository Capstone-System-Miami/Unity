using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class SlideShowIntro : MonoBehaviour
    {

        public Sprite[] slides;
        public Image slideshowImages;
        public CanvasGroup canvasGroup;
        public float slideDuration = 3f;
        public float fadeDuration = 1f;


        private int currentSlide = 0;




        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(PlaySlideShow());
        
        }

        IEnumerator PlaySlideShow()
        {
            while (currentSlide < slides.Length)
            {
                slideshowImages.sprite = slides[currentSlide];

                yield return StartCoroutine(Fade(0f, 1f)); // Start Fade

                yield return new WaitForSeconds(slideDuration);

                yield return StartCoroutine(Fade(1f, 0f)); // End fade

                currentSlide++;

            }


            Debug.Log("SlideShow Finished");
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
