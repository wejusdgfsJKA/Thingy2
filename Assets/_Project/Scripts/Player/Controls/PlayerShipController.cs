using TMPro;
using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(LineRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerShipController : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] Transform cam;
        [SerializeField] float rotationSpeed = 1, unlockedRotationSpeedMultiplier = 10;
        [Header("Movement")]
        [SerializeField] float maxThrust = 2.5f, minThrust = -1;
        [SerializeField] float thrustChangeRate = 0.1f;
        [SerializeField] float strafeSpeed = 1;
        [SerializeField] MoveInputReader moveInputReader;
        [SerializeField] Transform shipBody;
        [Header("UI")]
        [SerializeField] float directionLineMultiplier = 10;
        [SerializeField] TextMeshProUGUI thrustDisplay;
        Vector2 strafeVector;
        Vector3 rotateVector;
        float currentThrust;
        float thrustInput;
        bool followCamera = true;
        public float CurrentThrust
        {
            get => currentThrust;
            protected set
            {
                currentThrust = Mathf.Clamp(value, minThrust, maxThrust);
            }
        }
        LineRenderer lineRenderer;
        Rigidbody rb;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            if (!shipBody) shipBody = transform.root;
            if (!cam) cam = GetComponentInChildren<Camera>().transform;
        }
        private void OnEnable()
        {
            thrustDisplay.text = $"Thrust: {CurrentThrust:F1}";
            followCamera = true;
            strafeVector = rotateVector = Vector3.zero;
            CurrentThrust = 0;
            moveInputReader.EnablePlayerActions();
            moveInputReader.Strafe += OnStrafe;
            moveInputReader.Rotate += OnRotate;
            moveInputReader.Thrust += OnThrust;
            moveInputReader.FollowCameraToggle += OnFollowCameraToggle;
        }
        private void OnDisable()
        {
            moveInputReader.DisablePlayerActions();
        }
        void OnRotate(Vector3 vector3)
        {
            rotateVector = vector3;
        }
        void OnStrafe(Vector2 vector2)
        {
            strafeVector = vector2;
        }
        void OnThrust(float thrust)
        {
            thrustInput = thrust;
        }
        void OnFollowCameraToggle()
        {
            followCamera = !followCamera;
        }
        private void FixedUpdate()
        {
            #region Movement
            if (thrustInput != 0)
            {
                CurrentThrust += thrustInput * thrustChangeRate;
                thrustDisplay.text = $"Thrust: {CurrentThrust:F1}";
            }
            Vector3 velocity = strafeSpeed * (shipBody.up * strafeVector.y + shipBody.right * strafeVector.x)
                + CurrentThrust * shipBody.forward;
            rb.linearVelocity = velocity;

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + rb.linearVelocity * directionLineMultiplier);
            #endregion

            #region Rotation
            if (followCamera)
            {
                shipBody.rotation = Quaternion.Slerp(shipBody.rotation, cam.rotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                Quaternion yaw = Quaternion.AngleAxis(rotateVector.y, Vector3.up);
                Quaternion pitch = Quaternion.AngleAxis(rotateVector.x, Vector3.right);
                Quaternion roll = Quaternion.AngleAxis(rotateVector.z, Vector3.forward);

                //// Combine them in the right order: roll, then pitch, then yaw
                //targetRotation = targetRotation * yaw * pitch * roll;
                shipBody.localRotation = Quaternion.Slerp(shipBody.localRotation, shipBody.localRotation
                    * yaw * pitch * roll, unlockedRotationSpeedMultiplier * rotationSpeed * Time.fixedDeltaTime);
                //transform.localRotation = targetRotation;
            }
            #endregion
        }
    }
}