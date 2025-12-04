using Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
public static class GameManager
{
    public static float? Score { get; private set; } = null;
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
    public static void StartMission()
    {
        if (CurrentMission != null)
        {
            CurrentMission.Initialize();
            Score = null;
        }
    }
    public static void EndMission()
    {
        if (CurrentMission == null) return;
        Score = CurrentMission.GetScore();
        Debug.Log(Score);
        CurrentMission = null;
        Addressables.LoadSceneAsync(GlobalSettings.IntermediateSceneAddress);
    }
    static void AutoResolve() => EndMission();
}
