using UnityEngine;
namespace Player
{
    public abstract class InputReader : ScriptableObject
    {
        public static PlayerControls inputActions;
        public virtual void EnablePlayerActions()
        {
            inputActions ??= new PlayerControls();
            inputActions.Enable();
        }
        public virtual void DisablePlayerActions()
        {
            inputActions?.Disable();
        }
    }
}