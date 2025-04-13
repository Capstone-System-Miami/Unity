using UnityEngine;

namespace SystemMiami
{
    public class MusicPlayer : MonoBehaviour
    {
        public string soundName;
        // Start is called before the first frame update
        void Start()
        {
            AudioManager.MGR.Play(soundName);
        }
    }
}
