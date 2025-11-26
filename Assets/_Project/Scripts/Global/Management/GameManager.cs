using Player;
using UnityEngine;
public static class GameManager
{
    static PlayerShip player;
    public static PlayerShip Player
    {
        get => player;
        set
        {
            if (player != value)
            {
                player = value;
                if (player == null && CurrentMission != null) AutoResolve();
            }
        }
    }
    public static Mission CurrentMission { get; set; }

    public static void EndMission()
    {
        if (CurrentMission == null) return;
        var score = CurrentMission.GetScore();
        Debug.Log($"Score: {score}.");
        CurrentMission = null;
    }
    static void AutoResolve() => EndMission();
}
