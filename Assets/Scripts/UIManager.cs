using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VehicleCoinCollector
{
    /// <summary>
    /// UI Manager script managing HUD score, integrity bar, top-right Settings button,
    /// Settings modal popup, and Level Complete / Game Over modal popups.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        public TextMeshProUGUI scoreTMP;
        public Text scoreLegacyText;

        public Image healthBarFill;
        public TextMeshProUGUI healthTMP;
        public Text healthLegacyText;

        [Header("Top Right Settings Button")]
        public Button topRightSettingsButton;
        public GameObject settingsModalPanel;
        public Button closeSettingsButton;

        [Header("Level Complete Modal")]
        public GameObject levelCompletePanel;
        public TextMeshProUGUI finalScoreTMP;
        public Text finalScoreLegacyText;
        public Button restartButton;
        public Button quitButton;

        [Header("Game Over Modal")]
        public GameObject gameOverPanel;
        public TextMeshProUGUI gameOverTMP;
        public Text gameOverLegacyText;
        public Button gameOverRestartButton;

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
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (settingsModalPanel != null) settingsModalPanel.SetActive(false);

            if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
            if (gameOverRestartButton != null) gameOverRestartButton.onClick.AddListener(OnRestartClicked);

            if (topRightSettingsButton != null) topRightSettingsButton.onClick.AddListener(OpenSettingsModal);
            if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettingsModal);
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

        public void UpdateScoreUI(int currentCoins, int totalCoins)
        {
            string scoreString = $"Coins: {currentCoins} / {totalCoins}";

            if (scoreTMP != null) scoreTMP.text = scoreString;
            if (scoreLegacyText != null) scoreLegacyText.text = scoreString;
        }

        public void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            float fillRatio = Mathf.Clamp01((float)currentHealth / (float)maxHealth);
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = fillRatio;
            }

            string healthString = $"Integrity: {currentHealth} / {maxHealth}";

            if (healthTMP != null) healthTMP.text = healthString;
            if (healthLegacyText != null) healthLegacyText.text = healthString;
        }

        public void ShowLevelCompleteModal(int finalCoins, int totalCoins)
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }

            string summary = $"Level Complete!\nCoins Collected: {finalCoins} / {totalCoins}";

            if (finalScoreTMP != null) finalScoreTMP.text = summary;
            if (finalScoreLegacyText != null) finalScoreLegacyText.text = summary;
        }

        public void ShowGameOverModal()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            string summary = "VEHICLE BLASTED! 💥\nIntegrity reached 0!";

            if (gameOverTMP != null) gameOverTMP.text = summary;
            if (gameOverLegacyText != null) gameOverLegacyText.text = summary;
        }

        private void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartLevel();
            }
        }

        private void OnQuitClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }
}
