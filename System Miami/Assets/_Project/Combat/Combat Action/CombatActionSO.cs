using SystemMiami.CombatSystem;
using UnityEngine;

namespace SystemMiami.CombatRefactor
{
    public abstract class CombatActionSO : ScriptableObject
    {
        public Sprite Icon;
        public CombatSubactionSO[] Actions;
        public AnimatorOverrideController OverrideController;        
        public AnimatorOverrideController MageOverrideController;        
        public AnimatorOverrideController FighterOverrideController;        
        public AnimatorOverrideController TankOverrideController;        
        public AnimatorOverrideController RogueOverrideController;        
    }
}
