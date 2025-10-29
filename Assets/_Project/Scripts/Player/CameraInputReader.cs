using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "CameraInputReader", menuName = "ScriptableObjects/Player/CameraInputReader")]
    public class CameraInputReader : ScriptableObject, ICameraActions
    {
        public event UnityAction<Vector2> Move;
        public event UnityAction<float> Zoom;
        public PlayerControls inputActions;
        public Vector2 LookDirection => inputActions.Camera.Look.ReadValue<Vector2>();
        public void EnablePlayerActions()
        {
            inputActions ??= new PlayerControls();
            inputActions.Camera.SetCallbacks(this);
            inputActions.Enable();
        }
        public void DisablePlayerActions()
        {
            if (inputActions != null)
            {
                inputActions.Camera.SetCallbacks(null);
                inputActions.Disable();
            }
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }
        public void OnZoom(InputAction.CallbackContext context)
        {
            Zoom?.Invoke(context.ReadValue<float>());
        }
    }
}