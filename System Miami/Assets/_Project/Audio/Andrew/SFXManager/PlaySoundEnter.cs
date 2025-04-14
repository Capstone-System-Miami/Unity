using UnityEngine;

namespace SystemMiami
{
    public class PlaySoundEnter : StateMachineBehaviour
    {
        [SerializeField] private SoundType sound;
        [SerializeField, Range(0,1)] private float volume = 1;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SoundFxManager.MGR.PlaySound(sound, volume);
        }
    }
}
