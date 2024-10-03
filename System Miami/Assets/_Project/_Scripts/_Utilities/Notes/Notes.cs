using UnityEngine;

namespace SystemMiami
{
    public class Notes : MonoBehaviour
    {
        [SerializeField, TextArea(3, 20)]
        private string _notes;
    }
}
