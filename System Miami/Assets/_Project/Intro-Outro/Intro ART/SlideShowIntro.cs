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
        public float slideDuration = 3f;

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
                currentSlide++;
                yield return new WaitForSeconds(slideDuration);
            }


            Debug.Log("SlideShow Finished");
        }


    }
}
