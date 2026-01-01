using Global;
using Player;
using System.Collections.Generic;
using System.IO;
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
    public static void StartMission()
    {
        kills.Clear();
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
        CurrentPowerBalance += Score.GetValueOrDefault() + 25;
        CurrentMission = null;
        Teams[0] = Teams[1] = null;
        if (CurrentPowerBalance >= GlobalSettings.PlayerWinThreshold)
        {
            Addressables.LoadSceneAsync(GlobalSettings.EndSceneAddress);
            //the player has won
        }
        else if (CurrentPowerBalance <= GlobalSettings.PlayerLoseThreshold)
        {
            Addressables.LoadSceneAsync(GlobalSettings.EndSceneAddress);
            //the player has lost
        }
        else Addressables.LoadSceneAsync(GlobalSettings.IntermediateSceneAddress);
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
    public static void ExitToMenu()
    {
        ResumeGame();
        CurrentMission = null;
        SceneManager.LoadScene(0);
    }

    #region Player progress
    public static void BeginNewRun()
    {
        CurrentPowerBalance = 0;
        PlayerKills = 0;
    }
    static HashSet<int> kills = new();
    public static float CurrentPowerBalance { get; private set; } = 0;
    public static int PlayerKills { get; private set; } = 0;
    public static void ClearKill(Transform transform)
    {
        if (transform == null) return;
        kills.Remove(transform.GetInstanceID());
    }
    public static void AddPlayerKill(Transform victim)
    {
        if (victim == null) return;
        if (!kills.Contains(victim.GetInstanceID()))
        {
            PlayerKills++;
            kills.Add(victim.GetInstanceID());
        }
    }
    public static void DeleteCurrentSave()
    {
        File.Delete(GlobalSettings.GetSaveFilePath());
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
            PlayerKills = save.Value.PlayerKills;
            return true;
        }
        return false;
    }
    #endregion
}
