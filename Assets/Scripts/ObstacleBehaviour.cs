using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Obstacle behavior: supports stationary blocks and side-to-side moving/oscillating obstacles.
    /// Handles collision impact with player vehicle.
    /// </summary>
    public class ObstacleBehaviour : MonoBehaviour
    {
        public enum ObstacleType
        {
            Stationary,
            OscillatingSideToSide,
            OscillatingForwardBack
        }

        [Header("Obstacle Configuration")]
        public ObstacleType obstacleType = ObstacleType.Stationary;

        [Tooltip("Distance the obstacle moves back and forth.")]
        public float moveDistance = 4f;

        [Tooltip("Speed of oscillation.")]
        public float moveSpeed = 2f;

        [Tooltip("Damage dealt to vehicle on collision.")]
        public int damageToPlayer = 10;

        [Tooltip("Knockback force applied to vehicle upon impact.")]
        public float knockbackForce = 8f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            if (obstacleType == ObstacleType.OscillatingSideToSide)
            {
                float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
                transform.position = startPos + transform.right * offset;
            }
            else if (obstacleType == ObstacleType.OscillatingForwardBack)
            {
                float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
                transform.position = startPos + transform.forward * offset;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            PlayerVehicleController vehicle = collision.gameObject.GetComponentInParent<PlayerVehicleController>();
            if (vehicle != null)
            {
                // Play impact sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayObstacleHitSound();
                }

                // Apply damage
                vehicle.TakeIntegrityDamage(damageToPlayer);

                // Calculate pushback direction away from obstacle center and apply knockback
                Rigidbody vehicleRb = vehicle.GetComponent<Rigidbody>();
                if (vehicleRb != null)
                {
                    Vector3 knockbackDir = (collision.gameObject.transform.position - transform.position).normalized;
                    knockbackDir.y = 0.2f; // slight upward bounce
                    vehicleRb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}
