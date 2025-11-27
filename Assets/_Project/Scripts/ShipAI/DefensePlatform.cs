using Weapons;

public class DefensePlatform : Ship
{
    //field for a gun ig
    protected WeaponBase weapon;
    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponent<WeaponBase>();
    }
    public override void Tick()
    {
        //get the closest target
        Object closestTarget = null;
        float bestDist = float.PositiveInfinity;
        foreach (var target in Team.Targets[DetectionState.Identified])
        {

        }
        if (closestTarget != null)
        {
            weapon.Fire();
        }
    }
}
