using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerControls;
namespace Player
{
    [CreateAssetMenu(fileName = "MoveInputReader", menuName = "ScriptableObjects/PlayerShip/MoveInputReader")]
    public class MoveInputReader : InputReader, IMovementActions
    {
        public event System.Action<Vector2> Strafe;
        public event System.Action<Vector3> Rotate;
        public event System.Action<float> Thrust;
        public override void EnablePlayerActions()
        {
            base.EnablePlayerActions();
            inputActions.Movement.SetCallbacks(this);
        }
        public override void DisablePlayerActions()
        {
            base.DisablePlayerActions();
            Strafe = null;
            Rotate = null;
            Thrust = null;
            inputActions?.Movement.SetCallbacks(null);
        }
        public void OnRotate(InputAction.CallbackContext context)
        {
            Rotate?.Invoke(context.ReadValue<Vector3>());
        }

        public void OnStrafe(InputAction.CallbackContext context)
        {
            Strafe?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnThrust(InputAction.CallbackContext context)
        {
            Thrust?.Invoke(context.ReadValue<float>());
        }
    }
}