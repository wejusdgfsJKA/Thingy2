using UnityEngine;

public class Trackable : Registerable
{
    [field: SerializeField] public MeshRenderer MeshRenderer { get; protected set; }
    [field: SerializeField] public float TextSizeCoefficient { get; protected set; } = 5;
    public System.Action<Trackable> UpdateString;
    protected virtual void Awake() => MeshRenderer = GetComponentInChildren<MeshRenderer>();
    protected virtual void OnEnable()
    {
        ComponentManager<Trackable>.Register(this);
    }
    protected virtual void OnDisable()
    {
        ComponentManager<Trackable>.DeRegister(transform);
    }
}
