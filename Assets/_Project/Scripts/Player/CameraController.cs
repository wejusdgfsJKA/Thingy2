using UnityEngine;
namespace Player
{
    public class CameraController : MonoBehaviour
    {
        #region Fields
        float currentXAngle;
        float currentYAngle;

        public float cameraZoomSpeed = 1, cameraRotSpeed = 50f;
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] Transform camPivot, cam;
        [SerializeField] CameraInputReader inputReader;
        #endregion

        void Awake()
        {
            currentXAngle = transform.localRotation.eulerAngles.x;
            currentYAngle = transform.localRotation.eulerAngles.y;
        }
        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inputReader.EnablePlayerActions();
            inputReader.Zoom += OnZoom;
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            inputReader.DisablePlayerActions();
            inputReader.Zoom -= OnZoom;
        }

        void OnZoom(float zoom)
        {
            var oldZ = cam.transform.localPosition.z;
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x,
                cam.transform.localPosition.y, oldZ + zoom * cameraZoomSpeed);
        }

        void Update()
        {
            RotateCamera(inputReader.LookDirection.x, -inputReader.LookDirection.y);
        }

        void RotateCamera(float horizontalInput, float verticalInput)
        {
            if (smoothCameraRotation)
            {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }

            currentXAngle += verticalInput * cameraRotSpeed * Time.deltaTime;
            currentYAngle += horizontalInput * cameraRotSpeed * Time.deltaTime;

            camPivot.localRotation = Quaternion.Euler(currentXAngle, currentYAngle, 0);
        }
    }
}