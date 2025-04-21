using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class TestChangeSettings : MonoBehaviour
    {
        const float min = .0001f;
        const float max = 1;

        [SerializeField] private Slider musicVolSlider;
        [SerializeField] private Slider sfxVolSlider;

        private void Awake()
        {
            ClampSlider(musicVolSlider);
            ClampSlider(sfxVolSlider);
        }

        private void OnEnable()
        {
            musicVolSlider.value = AudioManager.MGR.GetMusicVolumePercent();
            sfxVolSlider.value = AudioManager.MGR.GetSfxVolumePercent();
        }

        private void Update()
        {
            AudioManager.MGR.AdjustMusicVolume(musicVolSlider.value);
        }

        private void ClampSlider(Slider slider)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }

        public void AdjustVolume(float percent)
        {
        }
    }
}
