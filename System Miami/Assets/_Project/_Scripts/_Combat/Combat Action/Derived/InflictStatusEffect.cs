using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "CombatAction/Inflict Status Effect")]
    public class InflictStatusEffect : CombatAction
    {
        [SerializeField] AttributeSetSO _effect;
        [SerializeField] int _turns;
        public override void SetTargeting()
        {
            _targeting = new Targeting(_user, this);
        }

        public override void Perform()
        {
            Combatant[] validTargets = _targetCombatants;
            AttributeSet effect = new AttributeSet(_effect);

            foreach (Combatant target in validTargets)
            {
                if (!target.TryGetComponent(out Attributes attr))
                {
                    Debug.Log($"Couldn't find attributes on the target");
                }
                else
                {
                    attr.AddStatusEffect(effect);
                }
            }
        }

    }
}
