using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Vehicle Movement & Physics Controller.
    /// Supports dynamic vehicle stats, raycast ground checks, jump mechanics, tire detachment, and explosion.
    /// </summary>
    public class PlayerVehicleController : MonoBehaviour
    {
        [Header("Vehicle Dynamics")]
        public float topSpeed = 20.0f;
        public float accelerationForce = 15.0f;
        public float turnSpeed = 80.0f;
        public float brakeForce = 25.0f;
        public float jumpForce = 9.0f;

        [Header("Integrity & Detachment")]
        public int maxIntegrity = 100;
        public int currentIntegrity = 100;
        public GameObject[] tireObjects;
        public GameObject roofObject;
        public GameObject explosionVFX;

        private Rigidbody rb;
        private bool isGrounded = false;
        private float lastHitTime = 0f;
        private int remainingTireCount = 4;
        private bool isExploded = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            ApplyUpgradedStats();
            currentIntegrity = maxIntegrity;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateIntegrityUI(currentIntegrity, maxIntegrity, remainingTireCount);
            }
        }

        public void ApplyUpgradedStats()
        {
            if (GarageManager.Instance != null && GarageManager.Instance.SelectedVehicle != null)
            {
                VehicleData v = GarageManager.Instance.SelectedVehicle;
                topSpeed = v.topSpeed * 3.2f;
                accelerationForce = v.acceleration * 2.8f;
                turnSpeed = v.handling * 14.0f;
                brakeForce = v.braking * 5.0f;
            }
        }

        private void FixedUpdate()
        {
            if (isExploded || (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing))
            {
                return;
            }

            CheckGrounded();

            // Get Input from InputManager
            float moveInput = (InputManager.Instance != null) ? InputManager.Instance.ThrottleInput : Input.GetAxis("Vertical");
            float turnInput = (InputManager.Instance != null) ? InputManager.Instance.SteeringInput : Input.GetAxis("Horizontal");
            bool jumpRequested = (InputManager.Instance != null) ? InputManager.Instance.JumpInput : Input.GetKeyDown(KeyCode.Space);

            // Wheel penalty based on detached tires
            float tireFactor = (remainingTireCount / 4.0f);
            float effectiveMaxSpeed = topSpeed * (0.3f + 0.7f * tireFactor);

            // Acceleration & Movement
            if (moveInput != 0f)
            {
                Vector3 moveDir = transform.forward * moveInput * accelerationForce;
                rb.AddForce(moveDir, ForceMode.Acceleration);
            }

            // Clamp max speed
            if (rb.linearVelocity.magnitude > effectiveMaxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * effectiveMaxSpeed;
            }

            // Steering
            if (Mathf.Abs(moveInput) > 0.05f || rb.linearVelocity.magnitude > 0.5f)
            {
                float turnDir = Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward));
                float turn = turnInput * turnSpeed * turnDir * Time.fixedDeltaTime;
                Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }

            // Jump
            if (jumpRequested && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayJumpSound();
            }
        }

        private void CheckGrounded()
        {
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, 0.6f);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isExploded || Time.time - lastHitTime < 1.0f) return;

            if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.GetComponent<ObstacleBehaviour>() != null)
            {
                lastHitTime = Time.time;
                TakeIntegrityDamage(10);
                if (AudioManager.Instance != null) AudioManager.Instance.PlayHitSound();

                // Knockback
                Vector3 knockbackDir = (transform.position - collision.contacts[0].point).normalized;
                rb.AddForce(knockbackDir * 6.0f, ForceMode.Impulse);
            }
        }

        public void TakeIntegrityDamage(int damage)
        {
            if (isExploded) return;

            currentIntegrity = Mathf.Max(0, currentIntegrity - damage);
            CheckTireDetachmentThresholds();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateIntegrityUI(currentIntegrity, maxIntegrity, remainingTireCount);
            }

            if (currentIntegrity <= 0)
            {
                ExplodeVehicle();
            }
        }

        private void CheckTireDetachmentThresholds()
        {
            int targetTires = 4;
            if (currentIntegrity <= 0) targetTires = 0;
            else if (currentIntegrity <= 20) targetTires = 1;
            else if (currentIntegrity <= 50) targetTires = 2;
            else if (currentIntegrity <= 70) targetTires = 3;

            while (remainingTireCount > targetTires && remainingTireCount > 0)
            {
                DetachOneTire();
            }
        }

        private void DetachOneTire()
        {
            if (remainingTireCount <= 0 || tireObjects == null || tireObjects.Length == 0) return;

            int tireIdx = 4 - remainingTireCount;
            if (tireIdx < tireObjects.Length && tireObjects[tireIdx] != null)
            {
                GameObject t = tireObjects[tireIdx];
                t.transform.SetParent(null);
                Rigidbody tireRb = t.GetComponent<Rigidbody>() ?? t.AddComponent<Rigidbody>();
                tireRb.mass = 2f;
                tireRb.AddForce(Vector3.up * 4f + Random.insideUnitSphere * 2f, ForceMode.Impulse);
                Destroy(t, 6.0f);
            }

            remainingTireCount--;
        }

        private void ExplodeVehicle()
        {
            if (isExploded) return;
            isExploded = true;

            if (AudioManager.Instance != null) AudioManager.Instance.PlayExplosionSound();

            // Detach remaining tires & roof
            while (remainingTireCount > 0) DetachOneTire();

            if (roofObject != null)
            {
                roofObject.transform.SetParent(null);
                Rigidbody roofRb = roofObject.GetComponent<Rigidbody>() ?? roofObject.AddComponent<Rigidbody>();
                roofRb.AddForce(Vector3.up * 7f + transform.forward * 3f, ForceMode.Impulse);
                Destroy(roofObject, 6.0f);
            }

            if (explosionVFX != null)
            {
                Instantiate(explosionVFX, transform.position + Vector3.up * 1.0f, Quaternion.identity);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver();
            }
        }
    }
}
