using Player;

public class Planet : Object
{
    static int count = 0;
    public static int Count
    {
        get => count;
        set
        {
            if (count != value)
            {
                count = value;
                if (count == 0) GameManager.EndMission();
            }
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Count++;
        ObjectDisplay.Instance.AddIdentified(this);
    }
    protected override void OnDisable()
    {
        ObjectDisplay.Instance.RemoveObject(this);
        Count--;
        base.OnDisable();
    }
}
