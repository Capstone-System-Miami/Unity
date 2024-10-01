// Author: Layla Hoey
using UnityEngine;
using SystemMiami.UI;

namespace SystemMiami
{
    [RequireComponent(typeof(Transformer))]
    public class DRIVER_Transformer : MonoBehaviour
    {
        [Header("Settings")]
        [Space(5)]
        [SerializeField] private Vector2 _targetVector;
        [SerializeField] private float _duration;
        [Space(10)]

        [Header("Triggers\n" +
            "\tClick a check box to perform an operation once")]
        [Space(5)]
        [SerializeField] private bool _repositionImmediate;
        [SerializeField] private bool _repositionOverTime;
        [SerializeField] private bool _revertPosition;
        [SerializeField] private bool _revertPositionOverTime;
        [SerializeField] private bool _resizeImmediate;
        [SerializeField] private bool _resizeOverTime;
        [SerializeField] private bool _revertSize;
        [SerializeField] private bool _revertSizeOverTime;

        private Transformer transformer;

        private void Start()
        {
            if (!TryGetComponent(out transformer))
            {
                print($"ERROR. Didn't find the transformer on {gameObject.name}");
            }
        }

        private void resetTriggers()
        {
            _repositionImmediate = false;
            _repositionOverTime = false;
            _revertPosition = false;
            _revertPositionOverTime = false;
            _resizeImmediate = false;
            _resizeOverTime = false;
            _revertSize = false;
            _revertSizeOverTime = false;
        }

        private void Update()
        {
            if (!transformer.IsBusy)
            {
                if (_repositionImmediate)
                {
                    transformer.Reposition(_targetVector);
                }
                else if (_repositionOverTime)
                {
                    transformer.Reposition(_targetVector, _duration);
                }
                else if (_revertPosition)
                {
                    transformer.RevertPosition();
                }
                else if (_revertPositionOverTime)
                {
                    transformer.RevertPosition(_duration);
                }
                else if (_resizeImmediate)
                {
                    transformer.Resize(_targetVector);
                }
                else if (_resizeOverTime)
                {
                    transformer.Resize(_targetVector, _duration);
                }
                else if (_revertSize)
                {
                    transformer.RevertSize();
                }
                else if (_revertSizeOverTime)
                {
                    transformer.RevertSize(_duration);
                }
            }

            resetTriggers();
        }
    }
}
