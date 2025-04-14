using UnityEngine.Audio;
using UnityEngine;

namespace SystemMiami
{
    [System.Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;
        public SoundType type;

        [Range(0f,1f)]
        public float volume;

        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        public Sound(string name, AudioClip clip, float volume, float pitch, bool loop)
        {
            this.name = name;
            this.clip = clip;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
        }

        public Sound(string name, SoundType type, float volume, float pitch, bool loop)
        {
            this.name = name;
            this.clip = null;
            this.type = type;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
        }
    }
}
