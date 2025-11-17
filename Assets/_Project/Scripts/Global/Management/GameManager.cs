using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameManager
{
    static Transform player;
    public static Transform Player
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
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Setup()
    {
        SceneManager.sceneLoaded += (s1, s2) =>
        {
            if (CurrentMission != null)
            {
                ObjectManager.Instance.SpawnPlayer();
                CurrentMission.Initialize();
            }
        };
    }
    public static void EndMission()
    {
        if (CurrentMission == null) return;
        var score = CurrentMission.GetScore();
        Debug.Log($"Score: {score}.");
        CurrentMission = null;
    }
    static void AutoResolve() => EndMission();
}
