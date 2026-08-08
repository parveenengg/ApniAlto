using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Trigger zone placed at the finish line area.
    /// Triggers Level Complete when the player vehicle enters.
    /// </summary>
    public class FinishZone : MonoBehaviour
    {
        private bool hasTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered) return;

            if (other.CompareTag("Player") || other.GetComponentInParent<PlayerVehicleController>() != null)
            {
                hasTriggered = true;

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayVictorySound();
                }

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnLevelComplete();
                }
            }
        }
    }
}
