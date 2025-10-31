using UnityEngine;
namespace Player
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] Vector3 speed = Vector3.one;
        [SerializeField] MoveInputReader moveInputReader;
        Vector3 strafeVector, rotateVector;
        Quaternion targetRotation;
        private void OnEnable()
        {
            strafeVector = rotateVector = Vector3.zero;
            moveInputReader.EnablePlayerActions();
            moveInputReader.Strafe += OnStrafe;
            moveInputReader.Rotate += OnRotate;
            targetRotation = transform.rotation;
        }
        private void OnDisable()
        {
            moveInputReader.DisablePlayerActions();
            moveInputReader.Strafe -= OnStrafe;
            moveInputReader.Rotate -= OnRotate;
        }
        void OnRotate(Vector3 vector3)
        {
            rotateVector = vector3;
        }
        void OnStrafe(Vector3 vector3)
        {
            strafeVector = vector3;
        }
        private void FixedUpdate()
        {
            #region Strafe
            transform.Translate(strafeVector);
            #endregion
            #region Rotate
            Quaternion yaw = Quaternion.AngleAxis(rotateVector.y, Vector3.up);
            Quaternion pitch = Quaternion.AngleAxis(rotateVector.x, Vector3.right);
            Quaternion roll = Quaternion.AngleAxis(rotateVector.z, Vector3.forward);

            // Combine them in the right order: roll, then pitch, then yaw
            targetRotation = targetRotation * yaw * pitch * roll;

            transform.localRotation = targetRotation;
            #endregion
        }
    }
}