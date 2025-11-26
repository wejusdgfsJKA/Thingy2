using System.Threading.Tasks;
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
        await Task.Delay(4000);
        SceneManager.LoadScene(1);
    }
}
