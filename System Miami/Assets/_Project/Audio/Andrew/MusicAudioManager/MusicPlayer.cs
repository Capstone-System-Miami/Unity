using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SystemMiami
{
    public class MusicPlayer : MonoBehaviour
    {
        AudioManager musicManager;
        public string name;
        // Start is called before the first frame update
        void Start()
        {
            musicManager = GetComponent<AudioManager>();
            musicManager.Play(name);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
