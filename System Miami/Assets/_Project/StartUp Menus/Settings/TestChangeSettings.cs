using UnityEngine;
using UnityEngine.UI;

namespace SystemMiami
{
    public class TestChangeSettings : MonoBehaviour
    {
        const float min = .0001f;
        const float max = 1;

        [SerializeField] private Slider mainVolSlider;
        [SerializeField] private Slider musicVolSlider;
        [SerializeField] private Slider sfxVolSlider;

        private void Awake()
        {
            ClampSlider(mainVolSlider);
            ClampSlider(musicVolSlider);
            ClampSlider(sfxVolSlider);
        }

        private void OnEnable()
        {
            mainVolSlider.value = AudioManager.MGR.GetMainVolumePercent();
            musicVolSlider.value = AudioManager.MGR.GetMusicVolumePercent();
            sfxVolSlider.value = AudioManager.MGR.GetSfxVolumePercent();
        }

        private void OnDisable()
        {
            mainVolSlider.value = AudioManager.MGR.GetMainVolumePercent();
            musicVolSlider.value = AudioManager.MGR.GetMusicVolumePercent();
            sfxVolSlider.value = AudioManager.MGR.GetSfxVolumePercent();
        }

        private void Update()
        {
            AudioManager.MGR.AdjustMainVolume(mainVolSlider.value);
            AudioManager.MGR.AdjustMusicVolume(musicVolSlider.value);
            AudioManager.MGR.AdjustSfxVolume(sfxVolSlider.value);
        }

        private void ClampSlider(Slider slider)
        {
            slider.minValue = min;
            slider.maxValue = max;
        }
    }
}
