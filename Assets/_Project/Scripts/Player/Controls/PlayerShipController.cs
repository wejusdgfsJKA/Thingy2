using TMPro;
using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class PlayerShipController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] int maxThrust = 10, minThrust = -1;
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
        public float CurrentThrust
        {
            get => currentThrust;
            protected set
            {
                currentThrust = Mathf.Clamp(value, minThrust, maxThrust);
            }
        }
        Quaternion targetRotation;
        LineRenderer lineRenderer;
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            shipBody ??= transform.root;
        }
        private void OnEnable()
        {
            strafeVector = rotateVector = Vector3.zero;
            CurrentThrust = 0;
            moveInputReader.EnablePlayerActions();
            moveInputReader.Strafe += OnStrafe;
            moveInputReader.Rotate += OnRotate;
            moveInputReader.Thrust += OnThrust;
            targetRotation = transform.rotation;
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
        private void FixedUpdate()
        {
            #region Movement
            CurrentThrust += thrustInput * thrustChangeRate;
            thrustDisplay.text = $"Thrust: {CurrentThrust:F1}";
            Vector3 velocity = strafeSpeed * (shipBody.up * strafeVector.y + shipBody.right * strafeVector.x).normalized
                + CurrentThrust * shipBody.forward;
            transform.Translate(velocity * Time.fixedDeltaTime, Space.World);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + velocity * directionLineMultiplier);
            #endregion
            #region Rotate
            //Quaternion yaw = Quaternion.AngleAxis(rotateVector.y, Vector3.up);
            //Quaternion pitch = Quaternion.AngleAxis(rotateVector.x, Vector3.right);
            //Quaternion roll = Quaternion.AngleAxis(rotateVector.z, Vector3.forward);

            //// Combine them in the right order: roll, then pitch, then yaw
            //targetRotation = targetRotation * yaw * pitch * roll;

            //transform.localRotation = targetRotation;
            #endregion
        }
    }
}