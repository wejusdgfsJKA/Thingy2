using HP;
using UnityEngine;

public class Ship : HPComponent
{
    [SerializeField] protected float speed;
    protected Navigation navigation;
    protected int metal;
    public int Metal
    {
        get; set;
    }
    protected int fuel;
    public int Fuel { get; set; }
    protected override void Awake()
    {
        base.Awake();
        navigation = new(transform, speed);
    }
    protected override void OnEnable()
    {
        metal = fuel = 0;
    }
    public void Repair(int amount)
    {

    }
    protected void Update()
    {
        navigation.Update(Time.deltaTime);
    }
}
