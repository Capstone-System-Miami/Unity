using UnityEngine;

namespace SystemMiami.AbilitySystem
{
    [System.Serializable]
    public class TargetingPattern
    {
        public int _radius;

        public bool _front;
        public bool _back;
        public bool _right;
        public bool _left;
        public bool _frontLeft;
        public bool _frontRight;
        public bool _backLeft;
        public bool _backRight;
    }
}
