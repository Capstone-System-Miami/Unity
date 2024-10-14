using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "CombatAction/Inflict Status Effect")]
    public class InflictStatusEffect : CombatAction
    {
        [SerializeField] AttributeSet effect;
        [SerializeField] IMovable move;
        [SerializeField] int turns;

        public override void PerformOn(GameObject target)
        {
            if (target.TryGetComponent(out Attributes attributes))
            {
                attributes.AddStatusEffect(effect);
            }
        }

        public override void PerformOn(GameObject me, GameObject target)
        {
        }
    }
}
