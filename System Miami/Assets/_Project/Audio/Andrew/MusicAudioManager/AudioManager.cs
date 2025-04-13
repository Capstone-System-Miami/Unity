using UnityEngine.Audio;
using UnityEngine;
using System;
using SystemMiami.Management;

namespace SystemMiami
{
    public class AudioManager : Singleton<AudioManager>
    {
        public Sound[] sounds;

        protected override void Awake()
        {
            base.Awake();

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;

                s.source.loop = s.loop;
            }
        }

        void Start()
        {
            //Play("OverWorld");
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " Not FOUND");
                return;
            }
            s.source.Play();
        }
    }
}
