using UnityEngine;

namespace VehicleCoinCollector
{
    /// <summary>
    /// Enum for control input types.
    /// </summary>
    public enum ControlType
    {
        SteeringWheel = 0,
        Buttons = 1,
        Tilt = 2
    }

    /// <summary>
    /// Unified input manager for cross-platform control handling (Keyboard, Touch Steering Wheel, Touch Buttons, Accelerometer/Tilt).
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [Header("Control Settings")]
        public ControlType ActiveControlType = ControlType.SteeringWheel;
        public float Sensitivity = 0.7f;
        public bool AutoAccelerate = false;

        // Dynamic State Values
        public float SteeringInput { get; private set; }
        public float ThrottleInput { get; private set; }
        public bool JumpInput { get; private set; }
        public bool HandbrakeInput { get; private set; }

        // Touch Input States
        private float touchSteerVal = 0f;
        private float touchThrottleVal = 0f;
        private bool touchJumpVal = false;
        private bool touchHandbrakeVal = false;

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

        private void Start()
        {
            if (SaveManager.Instance != null)
            {
                ActiveControlType = (ControlType)SaveManager.Instance.Data.controlType;
                Sensitivity = SaveManager.Instance.Data.sensitivity;
                AutoAccelerate = SaveManager.Instance.Data.autoAccelerateOn;
            }
        }

        private void Update()
        {
            // 1. Keyboard Controls (Desktop / WebGL)
            float keySteer = Input.GetAxis("Horizontal"); // A/D or Left/Right
            float keyThrottle = Input.GetAxis("Vertical"); // W/S or Up/Down
            bool keyJump = Input.GetKeyDown(KeyCode.Space);
            bool keyHandbrake = Input.GetKey(KeyCode.LeftShift);

            // 2. Accelerometer / Tilt Steering
            float tiltSteer = 0f;
            if (ActiveControlType == ControlType.Tilt)
            {
                tiltSteer = Mathf.Clamp(Input.acceleration.x * 2.5f * Sensitivity, -1f, 1f);
            }

            // 3. Combine Inputs
            if (Mathf.Abs(keySteer) > 0.05f)
            {
                SteeringInput = keySteer * Sensitivity;
            }
            else if (ActiveControlType == ControlType.Tilt && Mathf.Abs(tiltSteer) > 0.05f)
            {
                SteeringInput = tiltSteer;
            }
            else
            {
                SteeringInput = touchSteerVal * Sensitivity;
            }

            if (AutoAccelerate)
            {
                ThrottleInput = 1f;
            }
            else if (Mathf.Abs(keyThrottle) > 0.05f)
            {
                ThrottleInput = keyThrottle;
            }
            else
            {
                ThrottleInput = touchThrottleVal;
            }

            JumpInput = keyJump || touchJumpVal;
            HandbrakeInput = keyHandbrake || touchHandbrakeVal;

            // Reset single-frame touch triggers
            touchJumpVal = false;
        }

        // Public Methods for Touch UI Control Overlays
        public void SetTouchSteer(float val)
        {
            touchSteerVal = Mathf.Clamp(val, -1f, 1f);
        }

        public void SetTouchThrottle(float val)
        {
            touchThrottleVal = Mathf.Clamp(val, -1f, 1f);
        }

        public void TriggerTouchJump()
        {
            touchJumpVal = true;
        }

        public void SetTouchHandbrake(bool isPressed)
        {
            touchHandbrakeVal = isPressed;
        }

        public void SetControlType(ControlType newType)
        {
            ActiveControlType = newType;
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.Data.controlType = (int)newType;
                SaveManager.Instance.Save();
            }
        }
    }
}
