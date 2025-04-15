using UnityEngine;
using System;
using SystemMiami.Management;

namespace SystemMiami
{
    [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
    public class SoundFxManager : Singleton<SoundFxManager>
    {
        [SerializeField] private SoundList[] soundList;
        private AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(SoundType sound, float volume = 1)
        {
            AudioClip[] clips = soundList[(int)sound].Sounds;
            AudioClip randaomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
            //audioSource.pitch = UnityEngine.Random.Range(0, 3);
            audioSource.PlayOneShot(randaomClip, volume);
        }

        public void PlaySound(Sound sound)
        {
            if (sound.clip != null)
            {
                // audio source . play with all the info from 'sound'
            }
            else
            {
                AudioClip[] clipPool = soundList[(int)sound.type].Sounds;
                int randIndex = UnityEngine.Random.Range(0, clipPool.Length);

                // audio source . play with all the info from 'sound'
            }
        }

// TODO:
// Why is this inside a preprocessor directive?
// This means we don't want this to happen in actual builds.
#if UNITY_EDITOR
        // private void OnEnable()
        // {
        //     string[] names = Enum.GetNames(typeof(SoundType));
        //     Array.Resize(ref soundList, names.Length);
        //     for (int i = 0; i < soundList.Length; i++)
        //     {
        //         soundList[i].name = names[i];
        //     }
        // }
#endif
    }

    [Serializable]
    public struct SoundList
    {
        public AudioClip[] Sounds { get => sounds;  }
        [HideInInspector] public string name;
        [SerializeField] private AudioClip[] sounds;
    }

}
