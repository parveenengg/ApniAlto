using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Singleton score management script for tracking coin collection.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Stats")]
        public int currentCoins = 0;
        public int totalCoinsInLevel = 0;

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
            // Auto-count total coins placed in level if not set
            if (totalCoinsInLevel == 0)
            {
                totalCoinsInLevel = FindObjectsByType<Coin>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScoreUI(currentCoins, totalCoinsInLevel);
            }
        }

        public void AddCoin(int amount = 1)
        {
            currentCoins += amount;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScoreUI(currentCoins, totalCoinsInLevel);
            }
        }

        public void ResetScore()
        {
            currentCoins = 0;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateScoreUI(currentCoins, totalCoinsInLevel);
            }
        }
    }
}
