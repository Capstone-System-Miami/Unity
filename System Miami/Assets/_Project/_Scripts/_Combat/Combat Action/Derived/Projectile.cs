// Authors: Layla Hoey
using UnityEngine;

namespace SystemMiami.CombatSystem
{
    [CreateAssetMenu(fileName = "New Projectile Action", menuName = "CombatAction/Projectile")]
    public class Projectile : CombatAction
    {
        [SerializeField] int _range;

        public int Range { get { return _range; } }

        public override void SetTargeting()
        {
            _targeting = new Targeting(_user, this);
        }

        public override void Perform()
        {
            // don't know how this would work yet.
        }
    }
}
