using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using SystemMiami.Management;

namespace SystemMiami
{
    public enum SoundType
    {
        Sword,
        Magic,
        Hurt,
        Foorstep
    }

    public class AudioManager : Singleton<AudioManager>
    {
        public Sound[] sounds;

        protected override void Awake()
        {
            base.Awake();

            foreach (Sound sound in sounds)
            {
            }
        }

        public void PlaySound(Sound sound)
        {
            // Make sure the clip isn't null before we try to play it.
            if (sound.clip == null)
            {
                // If it is null, then we should use a random one of the same type instead.
                Sound[] soundPool = sounds.Where( s => (s.type == sound.type) ).ToArray();

                int randIndex = UnityEngine.Random.Range(0, soundPool.Length);

                // Reassign the arg 'sound' to be the new random sound we found.
                sound = soundPool[randIndex];
            }

            // If not null, we can use it as is,
            // so now we can be sure that our sound is usable.

            // New UNINSTANTIATED GameObject with an added AudioSource.
            // This is kind of like creating a prefab at runtime.
            // We can set the AudioSource's settings it before
            // we instantiate the object.
            GameObject sourceObj = new(sound.name);
            AudioSource source = sourceObj.AddComponent<AudioSource>();

            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;

            Instantiate(sourceObj);
            source.Play();

            // Define an action to perform in the future.
            // In this case, the action will be to destroy the object
            // we just created.
            Action onClipFinished = () => Destroy(source.gameObject);

            // We'll wait until the clip is finished, and then execute the
            // action we just defined.
            StartCoroutine(DoWhenOver(source, onClipFinished));
        }

        public void PlaySound(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " Not FOUND");
                return;
            }
        }

        private IEnumerator DoWhenOver(AudioSource sourceToCheck, Action onClipOver)
        {
            yield return new WaitUntil( () => !sourceToCheck.isPlaying );
            onClipOver.Invoke();
        }
    }
}
