using UnityEngine;
using UnityEngine.SceneManagement;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Game State Manager: handles level complete condition, game restart, and quit/menu navigation.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { Playing, LevelComplete, GameOver }

        [Header("State")]
        public GameState CurrentState { get; private set; } = GameState.Playing;

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
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.name);
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
