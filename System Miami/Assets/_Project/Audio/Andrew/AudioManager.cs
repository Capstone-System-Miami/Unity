using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SystemMiami
{
    public class AudioManager : MonoBehaviour
    {
        #region Values
        [SerializeField]

        [Header("Player")]
        public GameObject Player;
        [Header("Audio Clips")]
        public AudioSource Idle; // Player Idle
        public AudioSource Walking; // Player walking
        public AudioSource Enter; // Player Enter
        public AudioSource Exit; // Player Exit
        public AudioSource Shop; // Player Use Shop
        #endregion
        // Start is called before the first frame update
        void Start()
        {
            SetAudioLocation(Idle);
            SetAudioLocation(Walking);
            SetAudioLocation(Enter);
            SetAudioLocation(Exit);
            SetAudioLocation(Shop);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
        void SetAudioLocation(AudioSource ADS)
        {
            ADS.transform.position = Player.transform.position;
        }

    }
}
