using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Main Menu Manager: controls the Car Rush home screen, game mode selection,
    /// profile stats, currency displays, and navigation to settings/garage.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        [Header("Currency & Stats")]
        public int goldCoins = 12850;
        public int gems = 320;
        public int userLevel = 5;
        public int currentXP = 750;
        public int maxXP = 1200;

        [Header("UI Panels")]
        public GameObject mainHomeScreenPanel;
        public GameObject settingsModalPanel;

        [Header("Currency Text Displays")]
        public Text legacyCoinsText;
        public Text legacyGemsText;
        public Text legacyProfileText;

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
            UpdateMenuUI();
            if (settingsModalPanel != null) settingsModalPanel.SetActive(false);
        }

        public void UpdateMenuUI()
        {
            if (legacyCoinsText != null) legacyCoinsText.text = goldCoins.ToString("N0");
            if (legacyGemsText != null) legacyGemsText.text = gems.ToString("N0");
            if (legacyProfileText != null) legacyProfileText.text = $"Speedster07 (Lvl {userLevel})";
        }

        public void PlayFreePlayMode()
        {
            Time.timeScale = 1f;
            if (mainHomeScreenPanel != null) mainHomeScreenPanel.SetActive(false);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        public void OpenSettingsModal()
        {
            if (settingsModalPanel != null)
            {
                settingsModalPanel.SetActive(true);
            }
        }

        public void CloseSettingsModal()
        {
            if (settingsModalPanel != null)
            {
                settingsModalPanel.SetActive(false);
            }
        }

        public void ReturnToHomeScreen()
        {
            Time.timeScale = 0f;
            if (mainHomeScreenPanel != null)
            {
                mainHomeScreenPanel.SetActive(true);
            }
        }
    }
}
