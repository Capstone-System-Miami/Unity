// Authors: Layla Hoey, Lee St Louis
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    [RequireComponent(typeof(Attributes))]
    public class Stats : MonoBehaviour
    {
        #region VARS
        //===============================
        [SerializeField] private bool _printReports;

        // Collection of floats used in stat formulas
        [SerializeField] private StatData _statData;

        // For storing a reference to a
        // character's Attributes component
        private Attributes _attributes;

        // For storing our current, unmodified stats
        private StatSet _beforeEffects = new StatSet();

        // For storing our current stats,
        // as modified by any status effects
        private StatSet _afterEffects = new StatSet();

        // For any current status effects
        private Dictionary<StatSet, int> _statusEffects = new();

        //===============================
        #endregion // ^vars^

        #region PRIVATE METHODS
        //===============================

        private void Awake()
        {
            _attributes = GetComponent<Attributes>();
        }

        private void Start()
        {
            _beforeEffects = new StatSet(_attributes.GetSet(), _statData);
            updateEffects();
        }

        private void Update()
        {
            _beforeEffects = new StatSet(_attributes.GetSet(), _statData);
            updateEffects();
            if (_printReports) { print($"{name} stat: \n" +
                                        $"{getStatsReport()}"); }
        }

        /// <summary>
        /// Adds any values from status effects to
        /// _afterEffects, which should be the only
        /// source that any external scripts can use
        /// to read a Stat
        /// </summary>
        private void updateEffects()
        {
            for (int i = 0; i < CharacterEnums.STATS_COUNT; i++)
            {
                StatType stat = (StatType)i;

                _afterEffects.Set(stat, (_beforeEffects.GetStat(stat) + GetNetStatusEffects(stat)) );
            }
        }

        private string getStatsReport()
        {
            string result = "";

            for (int i = 0; i < CharacterEnums.STATS_COUNT; i++)
            {
                StatType stat = (StatType)i;

                result += $"{ stat }: \t { _afterEffects.GetStat(stat) }\n";
            }

            return result;
        }

        //===============================
        #endregion // ^private^

        #region PUBLIC METHODS
        //===============================

        /// <summary>
        /// Returns a given stat, including
        /// any mods made by active StatusEffects
        /// </summary>
        public float GetStat(StatType type)
        {
            return _afterEffects.GetStat(type);
        }

        // PREVIOUSLY LOCATED IN ATTRIBUTES
        #region Status Effects
        public void AddStatusEffect(StatSet effect, int durationTurns)
        {
            _statusEffects[effect] = durationTurns;
            Debug.Log($"{name} received a status effect for {durationTurns} turns.");
        }

        public float GetNetStatusEffects(StatType type)
        {
            float result = 0;

            foreach (StatSet statSet in _statusEffects.Keys)
            {
                result += statSet.GetStat(type);
            }

            return result;
        }

        public void DecrementStatusEffectDurations()
        {
            List<StatSet> toRemove = new();

            foreach (KeyValuePair<StatSet, int> entry in _statusEffects)
            {
                if (entry.Value <= 0)
                {
                    toRemove.Add(entry.Key);
                }
            }

            toRemove.ForEach(statSet => _statusEffects.Remove(statSet));
        }
        #endregion

        //===============================
        #endregion // ^public^
    }
}
