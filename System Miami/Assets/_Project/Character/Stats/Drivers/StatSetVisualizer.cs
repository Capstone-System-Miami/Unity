using System.Collections.Generic;
using SystemMiami.Utilities;
using UnityEngine;

namespace SystemMiami
{
    public class StatSetVisualizer : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Space(10)]
        [SerializeField] private List<LabeledField> fields = new();

        private StatSet statSet;

        private void Awake()
        {
            if (fields == null)
            {
                log.error($"Fields list was null.");
                return;
            }

            if (fields.Count != CharacterEnums.STATS_COUNT)
            {
                log.error($"Wrong number of fields in the fields list." +
                    $"Ensure that the number of fields is equivalent" +
                    $"to the number of Stat types.");
                return;
            }
        }

        private void Start()
        {
            SetLabels();
        }

        public void Assign(StatSet statSet)
        {
            this.statSet = statSet;
            SetValues();
        }

        private void SetLabels()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].Label.SetForeground(
                    CharacterEnums.STATS_NAMES[i]);
            }
        }

        private void SetValues()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].Value.SetForeground(
                    $"{statSet.GetStat((StatType)i)}");
            }
        }
    }
}
