using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Linq;
using SystemMiami.Management;
using SystemMiami.Utilities;
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
        const float MAX_DB_ATTENUATION = 20f;

        [Header("Debug")]
        public dbug log;

        [field: Header("Global")]
        [field: SerializeField] public AudioMixer mainMixer { get; private set; }

        [Header("Sound FX")]
        public Sound[] sounds;

        [field: Header("Music")]
        public Music mainMenu;
        public Music settings;
        public Music intro;
        public Music tutorial;
        public Music characterSelect;
        public Music neighborhood;
        public Music dungeonLow;
        public Music dungeonHigh;
        public Music outro;
        public Music credits;
        [field: SerializeField, ReadOnly] public AudioSource musicSource { get; private set; }

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

            // NOTE:
            // This array must be created before HandleSceneLoaded is called.
            allMusic = new Music[] {
                mainMenu,
                settings,
                characterSelect,
                neighborhood,
                dungeonLow,
                dungeonHigh
            };
        }

        private void OnEnable()
        {
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

            AudioSource source = CreateNewSource(sound);

            source.Play();

            // Define an action to perform in the future.
            Action onClipFinished = () => {
                // Anything we want to have happen when the 
                // AudioSource is not playing anything anymore:
                log?.print($"{sound.name}'s source is no longer playing anything.");
                Destroy(source.gameObject);
            };

            // We'll wait until the clip is finished, and then execute the
            // action we just defined.
            StartCoroutine( DoWhenOver(source, onClipFinished) );
        }

        public void PlaySound(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                log.warn("Sound: " + name + " Not FOUND");
                return;
            }

            PlaySound(s);
        }

        public void PlayMusic(Music music)
        {
            if (musicSource != null && musicSource.clip == music.clip)
            {
                log.print("Source clip was same. Returning.");
                return;
            }
            else if (musicSource != null)
            {
                log.print($"Source clip was not same. Destroying {musicSource.gameObject.name}");

                musicSource.Stop();
                Destroy(musicSource.gameObject);
            }

            log.print($"Creating new musicSource.");

            musicSource = CreateNewSource(music);
            musicSource.Play();
        }

        public void AdjustMusicVolume(float percent)
        {
            if (mainMixer == null) { Debug.LogError("didn't work"); return; }
            mainMixer.SetFloat("musicVolume", Mathf.Log10(percent) * MAX_DB_ATTENUATION);
        }
        public void AdjustSfxVolume(float percent)
        {
            if (mainMixer == null) { Debug.LogError("didn't work"); return; }

            mainMixer.SetFloat("musicVolume", Mathf.Log10(percent) * MAX_DB_ATTENUATION);
        }

        public float GetMusicVolumePercent()
        {
            mainMixer.GetFloat("musicVolume", out float currentAtten);
            return (Mathf.Pow(10, currentAtten)) / MAX_DB_ATTENUATION;
        }
        public float GetSfxVolumePercent()
        {
            mainMixer.GetFloat("sfxVolume", out float currentAtten);
            return (Mathf.Pow(10, currentAtten)) / MAX_DB_ATTENUATION;
        }


        protected virtual void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (overrideBuildIndex) { return; }

            sceneMusic = allMusic.Where(music => music.BuildIndex == scene.buildIndex).ToArray();

            log.print("Scene Music Arr", sceneMusic.Select(music => ( $"{music.name} (clip: '{music.clip}')" )).ToArray());
            Assert.IsNotNull(sceneMusic, $"{name}'s sceneMusic was null. Check HandleSceneLoaded()");
            Assert.IsTrue(sceneMusic.Length > 0, $"{name}'s sceneMusic had nothing in it. Check HandleSceneLoaded()");

            PlayMusic(sceneMusic[0]);
        }


        private IEnumerator DoWhenOver(AudioSource sourceToCheck, Action onClipOver)
        {
            yield return new WaitUntil( () => !sourceToCheck.isPlaying );
            onClipOver.Invoke();
        }

        /// <summary>
        /// Create a new INSTANTIATED GameObject with an added <see cref="AudioSource"/>.
        /// Sets the new AudioSource's settings to those of incomming param <see cref="Sound"/>.
        /// Sets the new AudioSource's <c>GameObject</c> as a child of this manager.
        /// NOTE: This does not call <c>Play()</c> on the new AudioSource.
        ///
        /// </summary>
        private AudioSource CreateNewSource(Sound sound)
        {
            GameObject sourceObj = new(sound.name);
            AudioSource newSource = sourceObj.AddComponent<AudioSource>();
            newSource.transform.SetParent(transform);

            newSource.outputAudioMixerGroup = sound.group;
            newSource.clip = sound.clip;
            newSource.volume = 1f;
            newSource.pitch = sound.pitch;
            newSource.loop = sound.loop;
            return newSource;
        }
    }
}
