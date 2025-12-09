using UnityEngine;
public class SignatureComponent : MonoBehaviour
{
    [SerializeField] protected float defaultSignature = 10;
    protected float signature;
    public float Signature
    {
        get => signature;
        protected set
        {
            signature = Mathf.Clamp(value, 0, float.MaxValue);
        }
    }
    private void Awake()
    {

    }
    private void OnEnable()
    {
        Signature = defaultSignature;
    }
    void ModifySignature(SignatureIncreaseEvent @event)
    {
        Signature += @event.Amount;
    }
}
