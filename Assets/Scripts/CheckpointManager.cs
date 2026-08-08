using UnityEngine;
using System.Collections.Generic;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Individual Checkpoint Ring trigger.
    /// </summary>
    public class CheckpointRing : MonoBehaviour
    {
        public int CheckpointIndex = 0;
        private bool isPassed = false;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetActiveRing(bool active)
        {
            if (meshRenderer != null)
            {
                meshRenderer.material.color = active ? new Color(0.13f, 0.77f, 0.36f, 0.8f) : new Color(0.5f, 0.5f, 0.5f, 0.3f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isPassed) return;
            if (other.CompareTag("Player") || other.GetComponentInParent<PlayerVehicleController>() != null)
            {
                if (CheckpointManager.Instance != null && CheckpointManager.Instance.OnCheckpointTriggered(CheckpointIndex))
                {
                    isPassed = true;
                    SetActiveRing(false);
                }
            }
        }
    }

    /// <summary>
    /// Sequential Checkpoint Manager tracking checkpoint progress (1/N -> N/N).
    /// </summary>
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        [Header("Checkpoints")]
        public List<CheckpointRing> Checkpoints = new List<CheckpointRing>();
        public int CurrentCheckpointIndex { get; private set; } = 0;
        public int TotalCheckpoints => Checkpoints.Count;

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
            ResetCheckpoints();
        }

        public void ResetCheckpoints()
        {
            CurrentCheckpointIndex = 0;
            for (int i = 0; i < Checkpoints.Count; i++)
            {
                if (Checkpoints[i] != null)
                {
                    Checkpoints[i].CheckpointIndex = i;
                    Checkpoints[i].SetActiveRing(i == 0);
                }
            }
            UpdateCheckpointUI();
        }

        public bool OnCheckpointTriggered(int index)
        {
            if (index == CurrentCheckpointIndex)
            {
                CurrentCheckpointIndex++;
                if (AudioManager.Instance != null) AudioManager.Instance.PlayCoinSound();

                if (CurrentCheckpointIndex < Checkpoints.Count)
                {
                    Checkpoints[CurrentCheckpointIndex].SetActiveRing(true);
                }

                UpdateCheckpointUI();

                if (CurrentCheckpointIndex >= Checkpoints.Count)
                {
                    // Completed all checkpoints!
                    if (RaceManager.Instance != null)
                    {
                        RaceManager.Instance.StopTimer();
                        RaceManager.Instance.SaveBestTime(RaceManager.Instance.RaceTimer);
                    }
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.OnLevelComplete();
                    }
                }
                return true;
            }
            return false;
        }

        private void UpdateCheckpointUI()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateCheckpointCounter(CurrentCheckpointIndex, TotalCheckpoints);
            }
        }
    }
}
