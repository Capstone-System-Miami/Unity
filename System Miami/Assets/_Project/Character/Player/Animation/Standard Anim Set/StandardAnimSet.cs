using UnityEngine;

namespace SystemMiami.Animation
{
    public class StandardAnimSet
    {
        public readonly AnimatorOverrideController immoble;
        public readonly AnimatorOverrideController idle;
        public readonly AnimatorOverrideController walking;
        public readonly AnimatorOverrideController takeDamage;

        public StandardAnimSet(
            AnimatorOverrideController immoble,
            AnimatorOverrideController idle,
            AnimatorOverrideController walking,
            AnimatorOverrideController takeDamage)
        {
            this.immoble = immoble;
            this.idle = idle;
            this.walking = walking;
            this.takeDamage = takeDamage;
        }
    }
}
