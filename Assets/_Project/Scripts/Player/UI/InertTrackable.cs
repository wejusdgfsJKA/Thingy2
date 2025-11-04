using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InertTrackable : Trackable
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
    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }
}
