using UnityEngine;

namespace SystemMiami.Animation
{
    [CreateAssetMenu(
        fileName = "New Standard Anim Set",
        menuName = "Animation/Standard Anim Set")]
    public class StandardAnimSetSO : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController immoble;
        [SerializeField] private AnimatorOverrideController idle;
        [SerializeField] private AnimatorOverrideController walking;
        [SerializeField] private AnimatorOverrideController takeDamage;

        public StandardAnimSet CreateSet()
        {
            return new StandardAnimSet(immoble, idle, walking, takeDamage);
        }
    }
}
