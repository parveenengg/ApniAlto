using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Collectible coin script: handles rotation animation and trigger pickup.
    /// </summary>
    public class Coin : MonoBehaviour
    {
        [Header("Coin Animation Settings")]
        [Tooltip("Degrees per second for Y-axis rotation.")]
        public float rotationSpeed = 120f;

        [Tooltip("Vertical bobbing amplitude.")]
        public float bobAmplitude = 0.2f;

        [Tooltip("Vertical bobbing frequency.")]
        public float bobFrequency = 2f;

        [Header("Coin Value")]
        public int coinValue = 1;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            // Continuous spinning animation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Gentle up and down floating animation
            float newY = startPosition.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if player vehicle touched the coin
            if (other.CompareTag("Player") || other.GetComponentInParent<PlayerVehicleController>() != null)
            {
                CollectCoin();
            }
        }

        private void CollectCoin()
        {
            // Play collection sound effect
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayCoinSound();
            }

            // Register score update
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddCoin(coinValue);
            }

            // Optional particle effect could spawn here

            // Disable or destroy coin object
            Destroy(gameObject);
        }
    }
}
