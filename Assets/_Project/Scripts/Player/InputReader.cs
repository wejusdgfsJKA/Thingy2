using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "ScriptableObjects/Player/InputReader")]
    public class InputReader : ScriptableObject, ICameraActions
    {
        public event UnityAction<Vector2> Move;
        public event UnityAction<float> Zoom;
        public event UnityAction Reset;
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
    }
}