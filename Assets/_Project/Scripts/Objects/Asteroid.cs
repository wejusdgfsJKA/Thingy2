using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Asteroid: Object
{
    protected Rigidbody rb;
    public Vector3 Inertia
    {
        get
        {
            return rb.linearVelocity;
        }
        set
        {
            rb.linearVelocity = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
}
