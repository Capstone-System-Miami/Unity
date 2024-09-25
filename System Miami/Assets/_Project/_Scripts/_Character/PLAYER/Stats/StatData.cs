using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class StatData
    {
        #region VARS
        //===============================

        [Header("Power (Physical & Magical)")]
        [SerializeField] private float _baseEffect = 2.5f;
        [SerializeField] private float _effectMultiplier = 1.5f;

        [Header("Ability Slots")]
        [SerializeField] private int _minSlots = 1;
        [SerializeField] private int _maxSlots = 6;
        [SerializeField] private int _slotAttributeThreshold = 5;
        [SerializeField] private float _slotMultiplier = 0.25f;

        [Header("Resources")]
        [SerializeField] private float _resourceMultiplier = 2f;

        [Header("Health")]
        [SerializeField] private float _healthMultiplier = 5f;
        [SerializeField] private int _healthAttributeThreshold = 10;

        [Header("Other")]
        [SerializeField] private float _damageReductionMultiplier = 1f;

        public float BaseEffect {  get { return _baseEffect; } }
        public float EffectMultiplier { get { return _effectMultiplier; } }
        public int MinSlots {  get { return _minSlots; } }
        public int MaxSlots { get { return _maxSlots; } }
        public int SlotAttributeThreshold { get { return _slotAttributeThreshold; } }
        public float SlotMultiplier { get { return _slotMultiplier; } }
        public float ResourceMultiplier { get { return _resourceMultiplier; } }
        public float HealthMultiplier { get { return _healthMultiplier; } }
        public int HealthAttributeThreshold { get { return _healthAttributeThreshold; } }
        public float DamageRdxMultiplier { get { return _damageReductionMultiplier; } }

        //===============================
        #endregion

        #region PRIVATE METHODS
        //===============================

        //===============================
        #endregion

        #region PUBLIC METHODS
        //===============================




        //===============================
        #endregion

    }
}
