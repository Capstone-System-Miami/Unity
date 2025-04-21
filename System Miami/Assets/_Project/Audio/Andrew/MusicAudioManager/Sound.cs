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
        public AudioMixerGroup group;

        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        // Default constructor, called when no arguments given,
        // or when you just declare the variable without a constuctor,
        // For example:
        // private Sound newSoundVar;  <- This line would call the default constructor.
        public Sound()
        {
            name = "Default Sound Name";
            clip = null;
            type = default;
            pitch = 1f;
            loop = false;
        }

        public Sound(AudioClip clip) : this()
        {
            this.clip = clip;
        }

        public Sound(SoundType type) : this()
        {
            this.type = type;
        }

        public Sound(string name, AudioClip clip, float volume, float pitch, bool loop)
        {
            this.name = name;
            this.clip = clip;
            this.pitch = pitch;
            this.loop = loop;
        }

        public Sound(string name, SoundType type, float volume, float pitch, bool loop)
        {
            this.name = name;
            this.clip = null;
            this.type = type;
            this.pitch = pitch;
            this.loop = loop;
        }
    }
}
