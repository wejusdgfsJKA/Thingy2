using UnityEngine;

public class Navigation
{
    protected Vector3? destination;
    public Vector3? Destination
    {
        get => destination;
        set
        {
            if (destination != value)
            {
                destination = value;
                if (destination == null) Rigidbody.linearVelocity = Vector3.zero;
            }
        }
    }
    public float Speed { get; set; }
    public float RotationSpeed { get; set; }
    public Transform Transform { get; set; }
    public Rigidbody Rigidbody { get; set; }
    public float RemainingDistance
    {
        get
        {
            return Destination.HasValue ? Vector3.Distance(Transform.position, Destination.Value) : 0;
        }
    }
    public bool UpdateRotation { get; set; } = true;
    /// <summary>
    /// The distance at which the ship will snap to the destination
    /// </summary>
    public float DestinationSnapDistance { get; set; }
    public Navigation(Transform tr, float speed, float rotationSpeed, float destinationSnapDistance = 0.1f)
    {
        Transform = tr;
        Rigidbody = tr.GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;
        Speed = speed;
        RotationSpeed = rotationSpeed;
        DestinationSnapDistance = destinationSnapDistance;
    }
    public void Rotate(Quaternion newRotation, float deltaTime)
    {
        Transform.rotation = Quaternion.Slerp(Transform.rotation, newRotation, RotationSpeed * deltaTime);
    }
    public void Update(float deltaTime)
    {
        if (Destination != null)
        {
            var dir = Destination.Value - Transform.position;
            if (dir.magnitude < DestinationSnapDistance)
            {
                Transform.position = Destination.Value;
                Destination = null;
                return;
            }
            Rigidbody.linearVelocity = dir.normalized * Speed;
            if (UpdateRotation)
            {
                Rotate(Quaternion.LookRotation(Destination.Value - Transform.position), deltaTime);
            }
        }
    }
    public void Stop() => Destination = null;
}