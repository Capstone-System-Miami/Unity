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

        public override void Perform(Targets targets)
        {
            AttributeSet effect = new AttributeSet(_effect);

            for(int i = 0; i < targets.Combatants.Length; i++)
            {
                if (!targets.Combatants[i].TryGetComponent(out Attributes attr))
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
