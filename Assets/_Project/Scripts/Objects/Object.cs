using Spawning.Pooling;
using UnityEngine;

public class Object : IDPoolable<ObjectType>
{
    #region Fields
    [field: SerializeField] public float Signature { get; set; } = 0;
    [field: SerializeField] public bool Persistent { get; set; } = false;
    [field: SerializeField] public bool Selectable { get; set; } = true;
    [field: SerializeField] public bool AlwaysVisible { get; protected set; }
    [field: SerializeField] public Sprite Icon { get; protected set; }
    [field: SerializeField] public Material Material { get; protected set; }
    [field: SerializeField] public float IconSizeCoefficient { get; protected set; } = 300;
    [field: SerializeField] public MeshRenderer IdentifiedRenderer { get; protected set; }
    [field: SerializeField] public MeshRenderer TrackedRenderer { get; protected set; }
    public event System.Action<Object> OnDespawn;
    public Transform Transform { get; protected set; }
    #endregion
    protected virtual void Awake()
    {
        Transform = transform;
        if (IdentifiedRenderer == null) IdentifiedRenderer = GetComponentInChildren<MeshRenderer>();
    }
    protected virtual void OnEnable()
    {
        if (ID == ObjectType.Player) return;
        IdentifiedRenderer.enabled = false;
        if (TrackedRenderer != null) TrackedRenderer.enabled = false;
        ObjectManager.Instance.Objects.Add(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        OnDespawn?.Invoke(this);
        ObjectManager.Instance.Objects.Remove(this);
    }
    protected virtual void OnDestroy()
    {
        OnDespawn = null;
    }
}
