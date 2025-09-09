using HP;
using UnityEngine;
public class DamageDealer : MonoBehaviour
{
    public bool b;
    public Transform target;
    public Collider coll;
    public int damage;
    public bool c;
    void Update()
    {
        if (b)
        {
            c = HPComponent.TakeDamage(target, new TakeDamage(damage, transform, coll));
            b = false;
        }
    }
}
