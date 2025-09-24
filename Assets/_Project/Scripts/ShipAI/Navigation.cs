using UnityEngine;

public class Navigation
{
    public Vector3? Destination { get; protected set; }
    public float Speed { get; set; }
    public Transform tr { get; set; }
    public float RemainingDistance
    {
        get
        {
            if (!Destination.HasValue) return 0;
            return Vector3.Distance(tr.position, Destination.Value);
        }
    }
    public void Update(float deltaTime)
    {
        if (Destination != null)
        {
            Vector3 dir = (Destination.Value - tr.position).normalized;

            tr.Translate(dir);
        }
    }
}