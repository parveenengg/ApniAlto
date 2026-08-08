using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Input Manager — Desktop keyboard input only.
    /// Mobile touch/tilt controls can be added later.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public float SteeringInput { get; private set; }
        public float ThrottleInput { get; private set; }
        public bool JumpInput { get; private set; }

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

        private void Update()
        {
            SteeringInput = Input.GetAxis("Horizontal");
            ThrottleInput = Input.GetAxis("Vertical");
            JumpInput = Input.GetKeyDown(KeyCode.Space);
        }
    }
}
