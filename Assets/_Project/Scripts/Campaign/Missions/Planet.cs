public class Planet : Trackable
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
    protected void OnEnable()
    {
        Count++;
        TrackableDisplay.Instance.AddTrackable(this);
    }
    protected override void OnDisable()
    {
        Count--;
        TrackableDisplay.Instance.RemoveTrackable(this);
        base.OnDisable();
    }
}
