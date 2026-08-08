using UnityEngine;
using System.Collections.Generic;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Vehicle controller with 100 max integrity, Space key jump capability (Raycast ground check),
    /// 10 damage per hit, hit cooldown, progressive tire detachment thresholds, and explosive blast.
    /// </summary>
    public class PlayerVehicleController : MonoBehaviour
    {
        [Header("Vehicle Dynamics")]
        public float moveSpeed = 12f;
        public float turnSpeed = 100f;
        public float brakeForce = 5f;
        public float jumpForce = 9.0f; // High vertical jump force to clear 2m-3m obstacles cleanly

        [Header("Vehicle Health & Integrity")]
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int damagePerHit = 10;

        [Header("Hit Invulnerability Cooldown")]
        public float hitCooldownDuration = 1.0f;
        private float lastHitTimestamp = -10f;

        [Header("Wheel Detachment System")]
        public Transform[] wheels;
        public float wheelRotationSpeed = 360f;
        private List<Transform> attachedWheels = new List<Transform>();
        private List<Transform> detachedWheels = new List<Transform>();

        [Header("Explosion Effects")]
        public Transform cabinRoof;
        public Transform bodyBase;

        private Rigidbody rb;
        private float moveInput;
        private float turnInput;
        private bool isExploded = false;
        private bool isGrounded = true;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.centerOfMass = new Vector3(0, -0.3f, 0);
            }
            currentHealth = maxHealth;

            if (wheels != null)
            {
                attachedWheels.AddRange(wheels);
            }
        }

        private void Start()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
            }
        }

        private void Update()
        {
            if (isExploded) return;

            // Ground check via downward raycast or height check
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f) || transform.position.y <= 0.65f;

            moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");

            // Press Space key to jump over obstacles!
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && isGrounded)
            {
                Jump();
            }

            if (attachedWheels.Count > 0 && Mathf.Abs(moveInput) > 0.05f)
            {
                float rotationAngle = moveInput * wheelRotationSpeed * Time.deltaTime;
                foreach (Transform wheel in attachedWheels)
                {
                    if (wheel != null)
                    {
                        wheel.Rotate(Vector3.right, rotationAngle, Space.Self);
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (isExploded) return;
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (rb == null) return;

            float wheelFactor = Mathf.Clamp01((float)attachedWheels.Count / 4f);
            float effectiveMoveSpeed = moveSpeed * (0.4f + 0.6f * wheelFactor);
            float effectiveTurnSpeed = turnSpeed * (0.3f + 0.7f * wheelFactor);

            if (Mathf.Abs(moveInput) > 0.05f)
            {
                Vector3 moveDirection = transform.forward * moveInput * effectiveMoveSpeed;
                Vector3 newVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, newVelocity, Time.fixedDeltaTime * 8f);

                float turnAngle = turnInput * effectiveTurnSpeed * Time.fixedDeltaTime * Mathf.Sign(moveInput);
                Quaternion turnRotation = Quaternion.Euler(0f, turnAngle, 0f);
                rb.MoveRotation(rb.rotation * turnRotation);
            }
            else
            {
                Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                horizontalVel = Vector3.Lerp(horizontalVel, Vector3.zero, Time.fixedDeltaTime * brakeForce);
                rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);
            }
        }

        private void Jump()
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJumpSound();
            }
        }

        public void TakeDamage(int customDamage = 10)
        {
            if (isExploded) return;

            if (Time.time - lastHitTimestamp < hitCooldownDuration) return;
            lastHitTimestamp = Time.time;

            int actualDamage = (customDamage > 0) ? customDamage : damagePerHit;
            currentHealth = Mathf.Max(0, currentHealth - actualDamage);

            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
            }

            CheckTireDetachmentThresholds();

            if (currentHealth <= 0)
            {
                ExplodeVehicle();
            }
        }

        private void CheckTireDetachmentThresholds()
        {
            int targetAttachedCount = 4;
            if (currentHealth <= 0) targetAttachedCount = 0;
            else if (currentHealth <= 20) targetAttachedCount = 1;
            else if (currentHealth <= 50) targetAttachedCount = 2;
            else if (currentHealth <= 70) targetAttachedCount = 3;

            while (attachedWheels.Count > targetAttachedCount && attachedWheels.Count > 0)
            {
                DetachNextWheel();
            }
        }

        private void DetachNextWheel()
        {
            if (attachedWheels.Count == 0) return;

            Transform wheelToDetach = attachedWheels[0];
            attachedWheels.RemoveAt(0);
            detachedWheels.Add(wheelToDetach);

            if (wheelToDetach != null)
            {
                wheelToDetach.SetParent(null, true);

                MeshCollider wheelCol = wheelToDetach.gameObject.AddComponent<MeshCollider>();
                wheelCol.convex = true;

                Rigidbody wheelRb = wheelToDetach.gameObject.AddComponent<Rigidbody>();
                wheelRb.mass = 25f;

                Vector3 popDirection = (wheelToDetach.position - transform.position).normalized + Vector3.up * 0.8f;
                wheelRb.AddForce(popDirection * Random.Range(6f, 10f), ForceMode.Impulse);
                wheelRb.AddTorque(Random.insideUnitSphere * 15f, ForceMode.Impulse);
            }
        }

        private void ExplodeVehicle()
        {
            if (isExploded) return;
            isExploded = true;

            while (attachedWheels.Count > 0)
            {
                DetachNextWheel();
            }

            DetachPartWithBlast(cabinRoof, 12f);
            DetachPartWithBlast(bodyBase, 8f);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayExplosionSound();
            }

            if (rb != null)
            {
                rb.isKinematic = true;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver();
            }
        }

        private void DetachPartWithBlast(Transform part, float blastForce)
        {
            if (part == null) return;
            part.SetParent(null, true);
            Rigidbody partRb = part.gameObject.AddComponent<Rigidbody>();
            partRb.mass = 40f;
            Vector3 blastDir = Vector3.up * 1.2f + Random.insideUnitSphere * 0.5f;
            partRb.AddForce(blastDir * blastForce, ForceMode.Impulse);
            partRb.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
        }

        public void ApplyKnockback(Vector3 forceDirection, float forceMagnitude)
        {
            if (rb != null && !isExploded)
            {
                rb.AddForce(forceDirection.normalized * forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
