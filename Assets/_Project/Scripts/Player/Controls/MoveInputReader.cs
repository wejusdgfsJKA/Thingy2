using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "MoveInputReader", menuName = "ScriptableObjects/Player/MoveInputReader")]
    public class MoveInputReader : InputReader, IMovementActions
    {
        public event System.Action<Vector3> Strafe, Rotate;
        public override void EnablePlayerActions()
        {
            base.EnablePlayerActions();
            inputActions.Movement.SetCallbacks(this);
        }
        public override void DisablePlayerActions()
        {
            base.DisablePlayerActions();
            inputActions?.Movement.SetCallbacks(null);
        }
        public void OnRotate(InputAction.CallbackContext context)
        {
            Rotate?.Invoke(context.ReadValue<Vector3>());
        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            Strafe?.Invoke(context.ReadValue<Vector3>());
        }
    }
}