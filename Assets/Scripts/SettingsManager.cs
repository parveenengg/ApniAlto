using UnityEngine;
using UnityEngine.UI;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Settings Manager handling master audio volume, SFX toggles, and settings modal UI interactions.
    /// </summary>
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [Header("Settings State")]
        public float masterVolume = 1.0f;
        public bool sfxEnabled = true;

        [Header("UI Controls")]
        public Slider volumeSlider;
        public Toggle sfxToggle;
        public GameObject settingsPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            if (volumeSlider != null)
            {
                volumeSlider.value = masterVolume;
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
            if (sfxToggle != null)
            {
                sfxToggle.isOn = sfxEnabled;
                sfxToggle.onValueChanged.AddListener(SetSFXEnabled);
            }
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public void OpenSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        public void CloseSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        public void ToggleSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
            }
        }

        public void SetVolume(float value)
        {
            masterVolume = value;
            AudioListener.volume = masterVolume;
        }

        public void SetSFXEnabled(bool enabled)
        {
            sfxEnabled = enabled;
        }
    }
}
