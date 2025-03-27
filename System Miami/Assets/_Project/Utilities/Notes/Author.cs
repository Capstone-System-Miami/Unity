using UnityEngine;

namespace SystemMiami
{
    public class Author : MonoBehaviour
    {
        [SerializeField] private string _name;

        public string Name {
            get { return _name; }
            set { _name = value; }
        }
    }
}
