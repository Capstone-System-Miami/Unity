// Author: Johnny Sosa
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class CameraShake : MonoBehaviour
    {

        public float shakeDuration = 0.15f;
        public float shakeMagnitude = 0.2f;

        private Vector3 originalPos;

        void Awake()
        {
            originalPos = transform.localPosition;  
        }

        public void Shake()
        {
            StartCoroutine(ShakeRoutine()); // Shake shake shake
        }

        IEnumerator ShakeRoutine()
        {
            
            float elapsed = 0.0f;

            while (elapsed < shakeDuration)
            {
                float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
                float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

                transform.localPosition = new Vector3(
                    originalPos.x + offsetX, // X changes
                    originalPos.y + offsetY, // Y changes
                    originalPos.z // Keeps Z the same
                );

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Reset position after shake
            transform.localPosition = originalPos;
        }
    }
}
