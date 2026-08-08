using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace VehicleCoinCollector
{
    /// <summary>
    /// UI Manager — HUD, pause modal, level complete modal, game over modal.
    /// Simplified for Free Play mode only.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI integrityText;
        public Image healthBarFill;
        public TextMeshProUGUI tireCountBadge;

        [Header("Modals")]
        public GameObject levelCompleteModal;
        public TextMeshProUGUI finalScoreText;
        public GameObject gameOverModal;
        public GameObject pauseModal;

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
            HideAllModals();
        }

        public void UpdateScoreUI(int currentCoins, int totalCoins)
        {
            if (scoreText != null)
            {
                scoreText.text = $"Coins: {currentCoins} / {totalCoins}";
            }
        }

        public void UpdateIntegrityUI(int currentHealth, int maxHealth, int tireCount)
        {
            if (integrityText != null)
            {
                integrityText.text = $"Integrity: {currentHealth} / {maxHealth}";
            }
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
            }
            if (tireCountBadge != null)
            {
                tireCountBadge.text = $"Tires: {tireCount}";
            }
        }

        public void UpdateCheckpointCounter(int current, int total)
        {
            // Placeholder for future checkpoint mode
        }

        public void ShowLevelCompleteModal(int coins, int totalCoins)
        {
            HideAllModals();
            if (levelCompleteModal != null) levelCompleteModal.SetActive(true);
            if (finalScoreText != null) finalScoreText.text = $"Coins Collected: {coins} / {totalCoins}";
        }

        public void ShowGameOverModal()
        {
            HideAllModals();
            if (gameOverModal != null) gameOverModal.SetActive(true);
        }

        public void ShowPauseModal()
        {
            HideAllModals();
            if (pauseModal != null) pauseModal.SetActive(true);
        }

        public void HidePauseModal()
        {
            if (pauseModal != null) pauseModal.SetActive(false);
        }

        public void HideAllModals()
        {
            if (levelCompleteModal != null) levelCompleteModal.SetActive(false);
            if (gameOverModal != null) gameOverModal.SetActive(false);
            if (pauseModal != null) pauseModal.SetActive(false);
        }

        // Button Callbacks
        public void OnClickRestart()
        {
            if (GameManager.Instance != null) GameManager.Instance.RestartLevel();
        }

        public void OnClickHomeMenu()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMenu();
            }
            else
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            }
        }

        public void OnClickResume()
        {
            if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
        }
    }
}
