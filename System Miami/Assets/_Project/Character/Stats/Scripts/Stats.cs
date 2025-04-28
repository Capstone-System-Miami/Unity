// Authors: Layla Hoey, Lee St Louis
using System.Collections.Generic;
using System.Linq;
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

        // Time-limited status effects: (StatSet, duration)
        private Dictionary<StatSet, int> _statusEffects = new();
       

        // Permanent equipment mods: (modID, StatSet)
        private Dictionary<int, StatSet> _equipmentMods = new();
        private StatSet TotalEquipmentEffects
        {
            get
            {
                StatSet runningTotal = new StatSet();
                _equipmentMods.Values.ToList().ForEach(mod => runningTotal += mod);
                return runningTotal;
            }
        }

        public StatSet BeforeEffectsCopy { get { return new(_beforeEffects); } }
        public StatSet AfterEffectsCopy { get { return new(_afterEffects); } }

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
            _beforeEffects = new StatSet(_attributes.GetAttributeSet(), _statData);
            updateEffects();
        }

        private void Update()
        {
            _beforeEffects = new StatSet(_attributes.GetAttributeSet(), _statData);
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
                //the after effect line was getting long so just made these into vars
                float baseValue      = _beforeEffects.GetStat(stat);
                float statusBonus    = GetNetStatusEffects(stat);
                float equipmentBonus = GetNetEquipmentMods(stat);

                _afterEffects.Set(stat, baseValue + statusBonus + equipmentBonus);
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
            // If there are no effects, just return
            if (_statusEffects.Count == 0)
            {
                Debug.Log($"{name}: No status effects to decrement.");
                return;
            }

            Debug.Log($"{name}: Starting with {_statusEffects.Count} status effects.");

            // Create a temporary list of the keys to avoid modifying during enumeration
            List<StatSet> keys = new List<StatSet>(_statusEffects.Keys);
            List<StatSet> toRemove = new List<StatSet>();

            foreach (StatSet entry in keys)
            {
                int currentDuration = _statusEffects[entry];
                int newDuration = currentDuration - 1;
                _statusEffects[entry] = newDuration;

                Debug.Log($"{name}: Effect {entry} duration decreased from {currentDuration} to {newDuration}");

                if (newDuration <= 0)
                {
                    Debug.Log($"{name}: Marking effect {entry} for removal (duration = {newDuration})");
                    toRemove.Add(entry);
                }
            }

            Debug.Log($"{name}: Found {toRemove.Count} effects to remove");

            // Remove expired effects
            foreach (StatSet statSet in toRemove)
            {
                bool removed = _statusEffects.Remove(statSet);
                Debug.Log($"{name}: Attempt to remove {statSet} effect: {(removed ? "Successful" : "FAILED")}");
            }

            Debug.Log($"{name}: After removal, {_statusEffects.Count} status effects remain.");
        }

        /// <summary>
        /// Adds an equipment mod's stats by ID, separate from status effects.
        /// </summary>
        public void EquipMod(int modID)
        {
            // Convert the ScriptableObject's bonuses into a runtime StatSet
            StatSet modStats = Database.MGR.GetEquipmentModStats(modID);

            if (_equipmentMods.ContainsKey(modID))
            {
                Debug.LogWarning($"{name} already has equipment mod {modID}!");
                return;
            }
            _equipmentMods.Add(modID, modStats);
            Debug.Log($"{name} equipped mod ID={modID}.");
        }

        /// <summary>
        /// Removes an equipment mod (if present).
        /// </summary>
        public void UnequipMod(int modID)
        {
            if (_equipmentMods.Remove(modID))
            {
                Debug.Log($"{name} unequipped mod ID={modID}.");
            }
            else
            {
                Debug.LogWarning($"{name} tried to unequip mod {modID}, but it wasn't found!");
            }
        }

        public float GetNetEquipmentMods(StatType stat)
        {
            float total = 0f;
            foreach (var kvp in _equipmentMods)
            {
                StatSet modStats = kvp.Value;
                total += modStats.GetStat(stat);
            }
            return total;
        }
        #endregion

        //===============================
        #endregion // ^public^
    }
}
