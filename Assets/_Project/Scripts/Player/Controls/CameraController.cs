using UnityEngine;
namespace Player
{
    public class CameraController : MonoBehaviour
    {
        #region Fields
        float currentXAngle;
        float currentYAngle;

        public float cameraZoomSpeed = 1, cameraRotSpeed = 50f;
        public Vector3 defaultCamPos = new(0, 0, 1);
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] Transform camPivot, cam;
        [SerializeField] CameraInputReader cameraInputReader;
        #endregion

        void Awake()
        {
            currentXAngle = transform.localRotation.eulerAngles.x;
            currentYAngle = transform.localRotation.eulerAngles.y;
        }
        private void OnEnable()
        {
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;
            cameraInputReader.EnablePlayerActions();
            cameraInputReader.Zoom += OnZoom;
            cameraInputReader.Reset += OnReset;
            OnReset();
        }
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraInputReader.DisablePlayerActions();
            cameraInputReader.Zoom -= OnZoom;
            cameraInputReader.Reset -= OnReset;
        }

        void OnZoom(float zoom)
        {
            var oldZ = cam.localPosition.z;
            cam.localPosition = new Vector3(cam.localPosition.x,
                cam.localPosition.y, oldZ + zoom * cameraZoomSpeed);
        }

        void OnReset()
        {
            camPivot.localRotation = Quaternion.identity;
            cam.localPosition = defaultCamPos;
        }

        void Update()
        {
            RotateCamera(cameraInputReader.LookDirection.x, -cameraInputReader.LookDirection.y);
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