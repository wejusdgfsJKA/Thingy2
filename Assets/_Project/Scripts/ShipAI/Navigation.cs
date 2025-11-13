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
                Rigidbody.linearVelocity = destination != null ?
                    (destination.Value - Transform.position).normalized * Speed :
                    Vector3.zero;
            }
        }
    }
    public float Speed { get; set; }
    public Transform Transform { get; set; }
    public Rigidbody Rigidbody { get; set; }
    public float RemainingDistance
    {
        get
        {
            return Destination.HasValue ? Vector3.Distance(Transform.position, Destination.Value) : 0;
        }
    }
    public Navigation(Transform tr, float speed)
    {
        Transform = tr;
        Rigidbody = tr.GetComponent<Rigidbody>();
        Speed = speed;
    }
    public void Update()
    {
        if (Destination != null)
        {
            var dir = Destination.Value - Transform.position;
            if (dir.magnitude <= Speed)
            {
                Transform.position = Destination.Value;
                Destination = null;
            }
        }
    }
}