using UnityEngine;
using System;

namespace VehicleCoinCollector
{
    public enum GameMode
    {
        FreePlay = 0,
        Checkpoints = 1,
        SprintMode = 2
    }

    /// <summary>
    /// Master manager for game modes (Free Play, Checkpoints, Sprint Mode), level timer, countdown, and map loading.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        [Header("Mode & Map State")]
        public GameMode CurrentMode = GameMode.FreePlay;
        public string CurrentMapId = "green_valley";

        [Header("Timer State")]
        public float RaceTimer { get; private set; } = 0f;
        public bool IsTimerRunning { get; private set; } = false;
        public float CountdownTimer { get; private set; } = 3.99f;
        public bool IsCountingDown { get; private set; } = false;

        public event Action<GameMode> OnGameModeChanged;
        public event Action<float> OnTimerUpdated;

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

        public void StartGameMode(GameMode mode, string mapId)
        {
            CurrentMode = mode;
            CurrentMapId = mapId;
            RaceTimer = 0f;
            IsTimerRunning = false;

            if (mode == GameMode.SprintMode || mode == GameMode.Checkpoints)
            {
                IsCountingDown = true;
                CountdownTimer = 3.99f;
            }
            else
            {
                IsCountingDown = false;
                IsTimerRunning = true;
            }

            OnGameModeChanged?.Invoke(mode);
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            {
                return;
            }

            if (IsCountingDown)
            {
                CountdownTimer -= Time.deltaTime;
                if (CountdownTimer <= 0f)
                {
                    IsCountingDown = false;
                    IsTimerRunning = true;
                    CountdownTimer = 0f;
                }
            }

            if (IsTimerRunning)
            {
                RaceTimer += Time.deltaTime;
                OnTimerUpdated?.Invoke(RaceTimer);
            }
        }

        public void StopTimer()
        {
            IsTimerRunning = false;
            IsCountingDown = false;
        }

        public void SaveBestTime(float time)
        {
            if (SaveManager.Instance == null) return;

            if (CurrentMode == GameMode.Checkpoints)
            {
                if (SaveManager.Instance.Data.bestTimeCheckpoints <= 0f || time < SaveManager.Instance.Data.bestTimeCheckpoints)
                {
                    SaveManager.Instance.Data.bestTimeCheckpoints = time;
                    SaveManager.Instance.Save();
                }
            }
            else if (CurrentMode == GameMode.SprintMode)
            {
                if (SaveManager.Instance.Data.bestTimeSprint <= 0f || time < SaveManager.Instance.Data.bestTimeSprint)
                {
                    SaveManager.Instance.Data.bestTimeSprint = time;
                    SaveManager.Instance.Save();
                }
            }
        }
    }
}
