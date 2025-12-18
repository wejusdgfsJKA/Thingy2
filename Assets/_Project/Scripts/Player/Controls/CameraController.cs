using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(ObjectDisplay))]
    public class CameraController : MonoBehaviour
    {
        #region Fields
        float currentXAngle;
        float currentYAngle;

        [SerializeField] protected float cameraZoomSpeed = 1, cameraDefaultRotSpeed = 30f;
        [SerializeField] protected float cameraRotSpeed;
        public Vector3 defaultCamPos = new(0, 0, 1);
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] Transform camPivot, cam, shipBody;
        [SerializeField] CameraInputReader cameraInputReader;
        protected ObjectDisplay objectDisplay;
        #endregion

        void Awake()
        {
            currentXAngle = camPivot.localRotation.eulerAngles.x;
            currentYAngle = camPivot.localRotation.eulerAngles.y;
            objectDisplay = GetComponent<ObjectDisplay>();
        }
        private void OnEnable()
        {
            DisableCursor();
            cameraInputReader.EnablePlayerActions();
            cameraInputReader.Zoom += OnZoom;
            cameraInputReader.Reset += OnReset;
            cameraInputReader.OnTargetSelect += objectDisplay.SelectTarget;
            OnReset();
        }
        private void OnDisable()
        {
            EnableCursor();
            cameraInputReader.DisablePlayerActions();
        }
        void DisableCursor()
        {
            cameraRotSpeed = cameraDefaultRotSpeed;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        void EnableCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnZoom(float zoom)
        {
            var oldZ = cam.localPosition.z;
            cam.localPosition = new Vector3(cam.localPosition.x,
                cam.localPosition.y, oldZ + zoom * cameraZoomSpeed);
        }

        void OnReset()
        {
            camPivot.localRotation = shipBody.localRotation;
            currentXAngle = camPivot.localRotation.eulerAngles.x;
            currentYAngle = camPivot.localRotation.eulerAngles.y;
            cam.localPosition = defaultCamPos;
        }

        void LateUpdate()
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