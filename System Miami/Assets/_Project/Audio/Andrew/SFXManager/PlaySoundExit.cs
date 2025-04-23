using UnityEngine;

namespace SystemMiami
{
    public class PlaySoundExit : StateMachineBehaviour
    {
        [SerializeField] private SoundType sound;
        [SerializeField, Range(0, 1)] private float volume = 1;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SoundFxManager.MGR.PlaySound(sound, volume);
        }
    }
}
