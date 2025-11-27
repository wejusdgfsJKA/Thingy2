using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        SceneManager.sceneLoaded += (s1, s2) =>
        {
            GameManager.CurrentMission?.Initialize();
        };
        await ObjectManager.LoadAssets();
        SceneManager.LoadScene(1);
    }
}
