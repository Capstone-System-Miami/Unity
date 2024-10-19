// Author: Layla Hoey
using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace SystemMiami.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Transformer : MonoBehaviour
    {
        #region VARS
        private RectTransform _transform;
        private Vector2 _positionBuffer;
        private Vector2 _sizeBuffer;

        private bool _isBusy;

        public bool IsBusy { get { return _isBusy; } }
        #endregion

        #region PRIVATE METHODS

        private void Awake()
        {
            if (!TryGetComponent(out _transform))
            {
                print($"ERROR. Didn't find the transform on {gameObject.name}");
            }
        }

        /// <summary>
        /// Sets the transform to the given _gridPosition (relative to parent & anchors), optionally storing the current one in a buffer
        /// </summary>
        private void reposition(Vector2 newPos, bool store)
        {
            _isBusy = true;
            if (store) { _positionBuffer = _transform.anchoredPosition; }

            _transform.anchoredPosition = newPos;
            _isBusy = false;
        }

        /// <summary>
        /// Swaps the current _gridPosition with the buffer
        /// </summary>
        private void reposition()
        {
            _isBusy = true;
            Vector2 temp = _positionBuffer;

            _positionBuffer = _transform.anchoredPosition;
            _transform.anchoredPosition = temp;
            _isBusy = false;
        }

        /// <summary>
        /// Sets the transform to the given size, optionally storing the current one in a buffer
        /// </summary>
        private void resize(Vector2 newSize, bool store)
        {
            _isBusy = true;
            if (store) { _sizeBuffer = _transform.sizeDelta; }

            _transform.sizeDelta = newSize;
            _isBusy = false;
        }

        /// <summary>
        /// Swaps the current size with the buffer
        /// </summary>
        private void resize()
        {
            _isBusy = true;
            Vector2 temp = _sizeBuffer;

            _sizeBuffer = _transform.sizeDelta;
            _transform.sizeDelta = temp;
            _isBusy = false;
        }

        /// <summary>
        /// Coroutine. Applies the change from the current _gridPosition to the [target pos vector] over the [duration].
        /// </summary>
        private IEnumerator changePosOverTime(Vector2 target, float duration)
        {
            _isBusy = true;
            Vector2 currentPos = _transform.anchoredPosition;
            reposition(currentPos, true);

            while (duration > 0)
            {
                currentPos = _transform.anchoredPosition;

                float estFramesLeft = duration / Time.deltaTime;
                Vector2 delta = (target - currentPos) / new Vector2(estFramesLeft, estFramesLeft);

                reposition(currentPos + delta, false);
                yield return new WaitForSeconds(Time.deltaTime);

                duration -= Time.deltaTime;
            }

            reposition(target, false);
            _isBusy = false;
        }

        /// <summary>
        /// Coroutine. Applies the change from the current size to the [target sz vector] over the [duration].
        /// </summary>
        private IEnumerator changeSizeOverTime(Vector2 target, float duration)
        {
            _isBusy = true;
            Vector2 currentSize = _transform.sizeDelta;
            resize(currentSize, true);

            while (duration > 0)
            {
                currentSize = _transform.sizeDelta;

                float estFramesLeft = duration / Time.deltaTime;
                Vector2 delta = (target - currentSize) / new Vector2(estFramesLeft, estFramesLeft);

                resize(currentSize + delta, false);
                yield return new WaitForSeconds(Time.deltaTime);

                duration -= Time.deltaTime;
            }

            resize(target, false);
            _isBusy = false;
        }
        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Immediately reposition the object
        /// </summary>
        public void Reposition(Vector2 newPos)
        {
            reposition(newPos, true);
        }

        /// <summary>
        /// Reposition the object over the span of a given duration.
        /// </summary>
        public void Reposition(Vector2 newPos, float duration)
        {
            StartCoroutine(changePosOverTime(newPos, duration));
        }

        /// <summary>
        /// Immediately revert the object to the last stored _gridPosition.
        /// </summary>
        public void RevertPosition()
        {
            reposition(_positionBuffer, true);
        }

        /// <summary>
        /// Revert the object to the last stored _gridPosition over the span of a given duration.
        /// </summary>
        public void RevertPosition(float duration)
        {
            StartCoroutine(changePosOverTime(_positionBuffer, duration));
        }

        /// <summary>
        /// Immediately resize the object.
        /// </summary>
        public void Resize(Vector2 newSize)
        {
            resize(newSize, true);
        }

        /// <summary>
        /// Resize the object over the span of a given duration.
        /// </summary>
        public void Resize(Vector2 newSize, float duration)
        {
            StartCoroutine(changeSizeOverTime(newSize, duration));
        }

        /// <summary>
        /// Immediately revert the object to the last stored size.
        /// </summary>
        public void RevertSize()
        {
            resize(_sizeBuffer, true);
        }

        /// <summary>
        /// Revert the object to the last stored _gridPosition over the span of a given duration.
        /// </summary>
        public void RevertSize(float duration)
        {
            StartCoroutine(changeSizeOverTime(_sizeBuffer, duration));
        }
        #endregion
    }
}
