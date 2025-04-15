using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using SystemMiami.Management;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

namespace SystemMiami
{
    public enum SoundType
    {
        Sword,
        Magic,
        Hurt,
        Footstep
    }

    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Sound FX")]
        public Sound[] sounds;

        [Header("Music")]
        public AudioSource musicSource;
        public Music mainMenu;
        public Music characterSelect;
        public Music neighborhood;
        public Music dungeonLow;
        public Music dungeonHigh;

        [Space(10)]
        public bool overrideBuildIndex;
        public Music overrideWith;

        private Music[] allMusic;
        private Music[] sceneMusic;

        protected override void Awake()
        {
            base.Awake();

            // If you need anything to happen in here,
            // just make sure you do it after the call to the base
            // version of awake above.

            musicSource = CreateNewMusicSource();
        }

        private void OnEnable()
        {
            // NOTE:
            // This array must be created before HandleSceneLoaded is called.
            allMusic = new Music[] {
                mainMenu,
                characterSelect,
                neighborhood,
                dungeonLow,
                dungeonHigh
            };

            SceneManager.sceneLoaded += HandleSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void Start()
        {
            if (overrideBuildIndex)
            {
                PlayMusic(overrideWith);
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

            source.Play();

            // Define an action to perform in the future.
            Action onClipFinished = () => {
                // Anything we want to have happen when the 
                // AudioSource is not playing anything anymore:
                Debug.Log($"{sound.name}'s source is no longer playing anything.");
                Destroy(source.gameObject);
            };

            // We'll wait until the clip is finished, and then execute the
            // action we just defined.
            StartCoroutine( DoWhenOver(source, onClipFinished) );
        }

        public void PlaySound(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " Not FOUND");
                return;
            }

            PlaySound(s);
        }

        public void PlayMusic(Sound music)
        {
            if (musicSource.isPlaying)
            {
                Destroy(musicSource.gameObject);
                musicSource = CreateNewMusicSource();
            }

            musicSource.clip = music.clip;
            musicSource.volume = music.volume;
            musicSource.pitch = music.pitch;
            musicSource.loop = music.loop;

            musicSource.Play();
        }

        protected virtual void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneMusic = allMusic.Where(music => music.BuildIndex == scene.buildIndex).ToArray();

            if (overrideBuildIndex) { return; }

            Assert.IsNotNull(sceneMusic, $"{name}'s sceneMusic was null. Check HandleSceneLoaded()");
            Assert.IsTrue(sceneMusic.Length > 0, $"{name}'s sceneMusic had nothing in it. Check HandleSceneLoaded()");

            if (musicSource != null && musicSource.isPlaying)
            {
                if (musicSource.clip != sceneMusic[0].clip)
                {
                    PlayMusic(sceneMusic[0]);
                }
            }
        }

        private IEnumerator DoWhenOver(AudioSource sourceToCheck, Action onClipOver)
        {
            yield return new WaitUntil( () => !sourceToCheck.isPlaying );
            onClipOver.Invoke();
        }

        private AudioSource CreateNewMusicSource()
        {
            GameObject sourceObj = new("Music");
            return sourceObj.AddComponent<AudioSource>();
        }
    }
}
