using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "CameraInputReader", menuName = "ScriptableObjects/Player/CameraInputReader")]
    public class CameraInputReader : InputReader, ICameraActions
    {
        public event UnityAction<Vector2> Move;
        public event UnityAction<float> Zoom;
        public event UnityAction Reset;
        public Vector2 LookDirection => inputActions.Camera.Look.ReadValue<Vector2>();
        public event UnityAction OnCursorEnabled;
        public event UnityAction OnCursorDisabled;
        public override void EnablePlayerActions()
        {
            base.EnablePlayerActions();
            inputActions.Camera.SetCallbacks(this);
            inputActions.Camera.EnableCursor.performed += EnableCursor;
            inputActions.Camera.EnableCursor.canceled += DisableCursor;
        }
        public override void DisablePlayerActions()
        {
            inputActions.Camera.EnableCursor.performed -= EnableCursor;
            inputActions.Camera.EnableCursor.canceled -= DisableCursor;
            OnCursorEnabled = OnCursorDisabled = Reset = null;
            Zoom = null;
            Move = null;
            inputActions?.Camera.SetCallbacks(null);
            base.DisablePlayerActions();
        }
        public void EnableCursor(InputAction.CallbackContext ctx) => OnCursorEnabled?.Invoke();
        public void DisableCursor(InputAction.CallbackContext ctx) => OnCursorDisabled?.Invoke();
        public void OnReset(InputAction.CallbackContext context)
        {
            Reset?.Invoke();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }
        public void OnZoom(InputAction.CallbackContext context)
        {
            Zoom?.Invoke(context.ReadValue<float>());
        }

        public void OnEnableCursor(InputAction.CallbackContext context)
        {

        }
    }
}