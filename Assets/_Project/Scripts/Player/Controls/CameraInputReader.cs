using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "CameraInputReader", menuName = "ScriptableObjects/PlayerShip/CameraInputReader")]
    public class CameraInputReader : InputReader, ICameraActions
    {
        public event UnityAction<Vector2> Move;
        public event UnityAction<float> Zoom;
        public event UnityAction Reset, ToggleMenu;
        public Vector2 LookDirection => inputActions.Camera.Look.ReadValue<Vector2>();
        public event UnityAction OnTargetSelect;
        public override void EnablePlayerActions()
        {
            base.EnablePlayerActions();
            inputActions.Camera.SetCallbacks(this);
        }
        public override void DisablePlayerActions()
        {
            Reset = OnTargetSelect = ToggleMenu = null;
            Zoom = null;
            Move = null;
            inputActions?.Camera.SetCallbacks(null);
            base.DisablePlayerActions();
        }
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
        public void OnSelectTarget(InputAction.CallbackContext context)
        {
            OnTargetSelect?.Invoke();
        }
        public void OnToggleMenu(InputAction.CallbackContext context)
        {
            ToggleMenu?.Invoke();
        }
    }
}