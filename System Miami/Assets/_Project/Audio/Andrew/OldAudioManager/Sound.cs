using UnityEngine.Audio;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class Sound
    {
        public enum Type { MUSIC, EFFECT } // for Master Audio Setting System
        public Type soundType;

        public string name;

        public AudioClip clip;

        [Range(0f,1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}
