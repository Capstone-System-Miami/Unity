using System.Collections.Generic;
using SystemMiami.Utilities;
using Unity.VisualScripting;
using UnityEngine;

namespace SystemMiami
{
    public class AttributeSetVisualizer : MonoBehaviour
    {
        [SerializeField] private dbug log;

        [Space(10)]
        [SerializeField] private List<LabeledField> fields = new();

        [SerializeField] private AttributeSet attributeSet;

        [SerializeField] private bool useEnumNames;

        private void OnEnable()
        {
            if (attributeSet == null)
            {
                attributeSet = PlayerManager.MGR.GetComponent<Attributes>().CurrentCopy;
                Assign(attributeSet);
                log.error($"NOT WORKING YIPPEEE");
                return;
            }
            if (fields == null)
            {
                log.error($"Fields list was null.");
                return;
            }

            if (fields.Count != CharacterEnums.ATTRIBUTE_COUNT)
            {
                log.error($"Wrong number of fields in the fields list." +
                    $"Ensure that the number of fields is equivalent" +
                    $"to the number of Attribute types.");
                return;
            }
        }

        private void Start()
        {
            if (useEnumNames)
            {
                SetLabels();
            }
        }

        public void Assign(AttributeSet attributeSet)
        {
            this.attributeSet = attributeSet;
            SetValues();
        }

        private void SetLabels()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                fields[i].Label.SetForeground(
                    CharacterEnums.ATTRIBUTE_NAMES[i] );
            }
        }

        private void SetValues()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                string msg = $"{attributeSet.Get((AttributeType)i)}";
                fields[i].Value.SetForeground(
                    $"{attributeSet.Get((AttributeType)i)}" );

            }
        }
    }
}
