using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Master UI Manager for canvas modals, HUD, mobile touch controls, checkpoint counter, sprint timer, pause menu, and results screen.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI integrityText;
        public Image healthBarFill;
        public TextMeshProUGUI tireCountBadge;
        public TextMeshProUGUI checkpointCounterText;
        public TextMeshProUGUI raceTimerText;
        public TextMeshProUGUI countdownText;

        [Header("Modals")]
        public GameObject levelCompleteModal;
        public TextMeshProUGUI finalScoreText;
        public GameObject gameOverModal;
        public GameObject pauseModal;
        public GameObject settingsModal;

        [Header("Mobile Touch Control Overlays")]
        public GameObject steeringWheelOverlay;
        public GameObject touchButtonsOverlay;

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
            UpdateTouchControlOverlays();
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
                tireCountBadge.text = $"🛞 {tireCount} Tires";
            }
        }

        public void UpdateCheckpointCounter(int current, int total)
        {
            if (checkpointCounterText != null)
            {
                checkpointCounterText.gameObject.SetActive(true);
                checkpointCounterText.text = $"CHECKPOINT {current} / {total}";
            }
        }

        public void UpdateRaceTimer(float timerSeconds)
        {
            if (raceTimerText != null)
            {
                raceTimerText.gameObject.SetActive(true);
                int minutes = Mathf.FloorToInt(timerSeconds / 60F);
                int seconds = Mathf.FloorToInt(timerSeconds % 60F);
                int fraction = Mathf.FloorToInt((timerSeconds * 100F) % 100F);
                raceTimerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
            }
        }

        public void UpdateCountdown(float secondsRemaining)
        {
            if (countdownText != null)
            {
                if (secondsRemaining > 0f)
                {
                    countdownText.gameObject.SetActive(true);
                    int sec = Mathf.CeilToInt(secondsRemaining);
                    countdownText.text = sec > 0 ? sec.ToString() : "GO!";
                }
                else
                {
                    countdownText.gameObject.SetActive(false);
                }
            }
        }

        public void ShowLevelCompleteModal(int coins, int totalCoins)
        {
            HideAllModals();
            if (levelCompleteModal != null)
            {
                levelCompleteModal.SetActive(true);
            }
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Coins Collected: {coins} / {totalCoins}";
            }
        }

        public void ShowGameOverModal()
        {
            HideAllModals();
            if (gameOverModal != null)
            {
                gameOverModal.SetActive(true);
            }
        }

        public void TogglePauseModal()
        {
            if (pauseModal != null)
            {
                bool isPaused = !pauseModal.activeSelf;
                pauseModal.SetActive(isPaused);
                Time.timeScale = isPaused ? 0f : 1f;
            }
        }

        public void ToggleSettingsModal()
        {
            if (settingsModal != null)
            {
                settingsModal.SetActive(!settingsModal.activeSelf);
            }
        }

        public void HideAllModals()
        {
            if (levelCompleteModal != null) levelCompleteModal.SetActive(false);
            if (gameOverModal != null) gameOverModal.SetActive(false);
            if (pauseModal != null) pauseModal.SetActive(false);
            if (settingsModal != null) settingsModal.SetActive(false);
        }

        public void UpdateTouchControlOverlays()
        {
            ControlType type = (InputManager.Instance != null) ? InputManager.Instance.ActiveControlType : ControlType.SteeringWheel;
            if (steeringWheelOverlay != null) steeringWheelOverlay.SetActive(type == ControlType.SteeringWheel);
            if (touchButtonsOverlay != null) touchButtonsOverlay.SetActive(type == ControlType.Buttons);
        }

        // Button Callbacks
        public void OnClickRestart()
        {
            if (GameManager.Instance != null) GameManager.Instance.RestartLevel();
        }

        public void OnClickHomeMenu()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null) GameManager.Instance.QuitGame();
        }
    }
}
