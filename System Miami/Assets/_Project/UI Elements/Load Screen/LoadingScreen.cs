using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace SystemMiami
{
    public class LoadingScreen : MonoBehaviour
    {
        [Header("Internal Refs")]
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private TMP_Text dotDotDot;

        [Header("Settings")]
        [SerializeField] private int maxDotRadius;
        [SerializeField] private float stepDuration;

        private void Start()
        {
            StartCoroutine(LoadCycle());
        }

        public IEnumerator LoadCycle()
        {
            const float MAX_DURATION = 10f;
            float timer = 0;
            while (timer < MAX_DURATION)
            {
                timer += Time.deltaTime;

                yield return StartCoroutine(DotCycle());
            }

            Debug.LogError($"LOADING TIMED OUT AFTER 10 SECONDS...");
        }

        public IEnumerator DotCycle()
        {
            int currentDotRadius = 0;

            do
            {
                int dotsToMake = 1 + (currentDotRadius * 2);
                Debug.Log($"{name} makin dots: {dotsToMake}");
                dotDotDot.text = MakeDots(dotsToMake);
                yield return new WaitForSeconds(stepDuration);
            } while (++currentDotRadius <= maxDotRadius);
        }

        string MakeDots(int amt)
        {
            string result = "";
            for (int i = 0; i < amt; i++)
            {
                result += '.';
            }
            return result;
        }

        // bool inRange(float step, float toCheck)
        // {
        //     return toCheck > step - tolerance && toCheck < step + tolerance;
        // }
    }
}
