using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Main Menu Manager: handles the home screen and launches the game.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        [Header("UI Panels")]
        public GameObject mainHomeScreenPanel;
        public GameObject settingsModalPanel;

        [Header("Currency Text Displays")]
        public Text legacyCoinsText;
        public Text legacyGemsText;
        public Text legacyProfileText;

        private const string GAME_SCENE_NAME = "VehicleCoinCollector";

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
            if (settingsModalPanel != null) settingsModalPanel.SetActive(false);

            // Load currency from SaveManager
            if (SaveManager.Instance != null)
            {
                if (legacyCoinsText != null) legacyCoinsText.text = SaveManager.Instance.Data.coins.ToString("N0");
                if (legacyGemsText != null) legacyGemsText.text = SaveManager.Instance.Data.gems.ToString("N0");
                if (legacyProfileText != null) legacyProfileText.text = $"Speedster07 (Lvl {SaveManager.Instance.Data.playerLevel})";
            }
        }

        /// <summary>
        /// Launch the game — Free Play on Green Valley.
        /// </summary>
        public void PlayGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(GAME_SCENE_NAME);
        }

        public void OpenSettingsModal()
        {
            if (settingsModalPanel != null) settingsModalPanel.SetActive(true);
        }

        public void CloseSettingsModal()
        {
            if (settingsModalPanel != null) settingsModalPanel.SetActive(false);
        }
    }
}
