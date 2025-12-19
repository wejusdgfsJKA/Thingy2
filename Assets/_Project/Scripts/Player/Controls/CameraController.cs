using Player.UI;
using UnityEngine;
namespace Player
{
    [RequireComponent(typeof(ObjectDisplay))]
    [RequireComponent(typeof(PauseMenuManager))]
    public class CameraController : MonoBehaviour
    {
        #region Fields
        float currentXAngle;
        float currentYAngle;

        [SerializeField] protected float cameraZoomSpeed = 1;
        [SerializeField] protected float cameraRotSpeed = 30;
        public Vector3 defaultCamPos = new(0, 0, 1);
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        [SerializeField] Transform camPivot, cam, shipBody;
        [SerializeField] CameraInputReader cameraInputReader;
        protected ObjectDisplay objectDisplay;
        PauseMenuManager pauseMenuManager;
        #endregion

        #region Setup
        void Awake()
        {
            currentXAngle = camPivot.localRotation.eulerAngles.x;
            currentYAngle = camPivot.localRotation.eulerAngles.y;
            objectDisplay = GetComponent<ObjectDisplay>();
            pauseMenuManager = GetComponent<PauseMenuManager>();
        }
        private void OnEnable()
        {
            GameManager.ResumeGame();
            cameraInputReader.EnablePlayerActions();
            cameraInputReader.Zoom += OnZoom;
            cameraInputReader.Reset += OnReset;
            cameraInputReader.ToggleMenu += OnMenu;
            cameraInputReader.OnTargetSelect += objectDisplay.SelectTarget;
            OnReset();
        }
        private void OnDisable()
        {
            cameraInputReader.DisablePlayerActions();
        }
        #endregion

        #region Camera movement
        void OnZoom(float zoom)
        {
            if (GameManager.IsPaused) return;
            var oldZ = cam.localPosition.z;
            cam.localPosition = new Vector3(cam.localPosition.x,
                cam.localPosition.y, oldZ + zoom * cameraZoomSpeed);
        }

        void OnReset()
        {
            if (GameManager.IsPaused) return;
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
            if (GameManager.IsPaused) return;

            if (smoothCameraRotation)
            {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }

            currentXAngle += verticalInput * cameraRotSpeed * Time.deltaTime;
            currentYAngle += horizontalInput * cameraRotSpeed * Time.deltaTime;

            camPivot.localRotation = Quaternion.Euler(currentXAngle, currentYAngle, 0);
        }
        #endregion

        public void OnMenu()
        {
            GameManager.TogglePause();
            pauseMenuManager.ToggleMenu();
        }
    }
}