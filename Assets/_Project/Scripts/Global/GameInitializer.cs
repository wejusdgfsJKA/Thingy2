using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    async void Start()
    {
        SceneManager.sceneLoaded += (s1, s2) =>
        {
            GameManager.StartMission();
        };
        var t1 = ObjectManager.LoadAssets();
        var t2 = Weapons.WeaponManager.LoadAssets();
        await Task.WhenAll(t1, t2);
        SceneManager.LoadScene(1);
    }
}
