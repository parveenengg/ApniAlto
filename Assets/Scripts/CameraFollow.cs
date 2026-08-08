using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Smooth third-person trailing camera controller following the player vehicle.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target to Follow")]
        public Transform target;

        [Header("Camera Offset Settings")]
        [Tooltip("Offset relative to vehicle position and orientation.")]
        public Vector3 offset = new Vector3(0f, 6f, -10f);

        [Tooltip("Camera follow movement smoothness.")]
        public float followSpeed = 8f;

        [Tooltip("Camera rotation look-at speed.")]
        public float rotationSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            // Calculate desired target position taking target orientation into account
            Vector3 targetPosition = target.TransformPoint(offset);

            // Smooth position movement
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

            // Smoothly look towards the target vehicle
            Vector3 lookTargetPos = target.position + Vector3.up * 1.5f;
            Quaternion targetRotation = Quaternion.LookRotation(lookTargetPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
