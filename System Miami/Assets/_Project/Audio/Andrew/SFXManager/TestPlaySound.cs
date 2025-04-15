using UnityEngine;

namespace SystemMiami
{
    public class TestPlaySound : MonoBehaviour
    {
        [SerializeField] public SoundType sound;
        [SerializeField, Range(0,1)] public float value = 1;
        // Start is called before the first frame update
        // void Start()
        // {
        //     AudioManager.MGR.PlaySound(sound);
        // }
    }
}
