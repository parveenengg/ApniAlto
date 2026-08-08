using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Race Manager — Free Play mode only.
    /// More game modes (Checkpoints, Sprint) can be added later.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [Header("Timer")]
        public float RaceTimer { get; private set; } = 0f;
        public bool IsTimerRunning { get; private set; } = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Start free play mode — timer runs, no countdown.
        /// </summary>
        public void StartFreePlay()
        {
            RaceTimer = 0f;
            IsTimerRunning = true;
            Debug.Log("[RaceManager] Free Play started.");
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            {
                return;
            }

            if (IsTimerRunning)
            {
                RaceTimer += Time.deltaTime;
            }
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
        }
    }
}
