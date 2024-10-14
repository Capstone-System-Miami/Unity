using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "CombatAction/Cause Status Effect")]
    public class CauseStatusEffect : CombatAction
    {
        [SerializeField] AttributeSet effect;
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
