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
                if (player == null && CurrentMission != null)
                {
                    AutoResolve();
                }
            }
        }
    }
    public static Mission CurrentMission { get; set; }
    /// <summary>
    /// Teams[0] is the player's team, Teams[1] is the enemy team.
    /// </summary>
    public static Team[] Teams { get; private set; } = new Team[2];
    public static void StartMission()
    {
        if (CurrentMission != null)
        {
            Teams[0] = new(0); Teams[1] = new(1);
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
        Teams[0] = Teams[1] = null;
        Addressables.LoadSceneAsync(GlobalSettings.IntermediateSceneAddress);
    }
    public static void AutoResolve() => EndMission();
    public static bool IsPaused => Time.timeScale == 0f;
    public static void TogglePause()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = IsPaused ? CursorLockMode.Locked : CursorLockMode.None;
        Time.timeScale = IsPaused ? 1f : 0f;
    }
    public static void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
