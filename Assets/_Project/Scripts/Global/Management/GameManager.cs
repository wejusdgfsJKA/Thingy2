using Global;
using Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
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
    public static float CurrentPowerBalance { get; private set; } = 0;
    public static void StartMission()
    {
        if (CurrentMission != null)
        {
            Teams[0] = new(0); Teams[1] = new(1);
            CurrentMission.Initialize();
            Score = null;
        }
    }
    public static void BeginNewRun()
    {
        CurrentPowerBalance = 0;
    }
    public static void EndMission()
    {
        if (CurrentMission == null) return;
        Score = CurrentMission.GetScore();
        CurrentPowerBalance += Score.GetValueOrDefault();
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
    public static void Save(string fileName)
    {
        GameSave.Save(GlobalSettings.GetSaveFilePath(fileName));
    }
    public static bool Load(string fileName)
    {
        var save = GameSave.Load(GlobalSettings.GetSaveFilePath(fileName));
        if (save != null)
        {
            CurrentPowerBalance = save.Value.PowerBalance;
            return true;
        }
        return false;
    }
    public static void ExitToMenu()
    {
        ResumeGame();
        CurrentMission = null;
        SceneManager.LoadScene(0);
    }
}
