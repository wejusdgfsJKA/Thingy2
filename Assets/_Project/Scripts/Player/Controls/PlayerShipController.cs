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
        [SerializeField] float thrust = 2, maxVelocityMagnitude = 2;
        [SerializeField] float strafeSpeed = 1;
        [SerializeField] MoveInputReader moveInputReader;
        [SerializeField] Transform shipBody;
        [Header("UI")]
        [SerializeField] float directionLineMultiplier = 10;
        Vector2 strafeVector;
        Vector3 rotateVector;
        float currentThrust;
        float thrustInput;
        bool followCamera = true;
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
            followCamera = true;
            strafeVector = rotateVector = Vector3.zero;
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

            Vector3 velocity = strafeSpeed * Time.fixedDeltaTime * (shipBody.up * strafeVector.y + shipBody.right * strafeVector.x)
                + thrust * thrustInput * shipBody.forward;
            rb.linearVelocity += velocity;
            if (rb.linearVelocity.magnitude > maxVelocityMagnitude)
            {
                rb.linearVelocity = maxVelocityMagnitude * rb.linearVelocity.normalized;
            }

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