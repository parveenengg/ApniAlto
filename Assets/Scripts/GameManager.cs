using UnityEngine;
using UnityEngine.SceneManagement;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Game State Manager: handles level complete condition, game restart, pause toggle,
    /// return to menu, and quit/menu navigation.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { Playing, LevelComplete, GameOver, Paused }

        [Header("State")]
        public GameState CurrentState { get; private set; } = GameState.Playing;

        private GameState stateBeforePause = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Time.timeScale = 1f;
        }

        private void Update()
        {
            // Escape key toggles pause on desktop
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CurrentState == GameState.Paused)
                {
                    ResumeGame();
                }
                else if (CurrentState == GameState.Playing)
                {
                    PauseGame();
                }
            }
        }

        public void PauseGame()
        {
            if (CurrentState != GameState.Playing) return;

            stateBeforePause = CurrentState;
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPauseModal();
            }
        }

        public void ResumeGame()
        {
            if (CurrentState != GameState.Paused) return;

            CurrentState = stateBeforePause;
            Time.timeScale = 1f;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseModal();
            }
        }

        public void OnLevelComplete()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.LevelComplete;
            int finalCoins = (ScoreManager.Instance != null) ? ScoreManager.Instance.currentCoins : 0;
            int totalCoins = (ScoreManager.Instance != null) ? ScoreManager.Instance.totalCoinsInLevel : 0;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowLevelCompleteModal(finalCoins, totalCoins);
            }

            Time.timeScale = 0f; // Pause physics/game loop
        }

        public void OnGameOver()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.GameOver;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowGameOverModal();
            }

            Time.timeScale = 0f;
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            CurrentState = GameState.Playing;
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
        }

        /// <summary>
        /// Return to the main menu scene (GetStarted_Scene at index 0).
        /// </summary>
        public void ReturnToMenu()
        {
            Time.timeScale = 1f;
            CurrentState = GameState.Playing;
            SceneManager.LoadScene(0);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
