using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStart()
    {
        GameManager.CurrentMission = new PlanetDefenseMission(1);
        if (GameManager.CurrentMission != null) SceneManager.LoadScene(2);
    }
    public void OnExit()
    {
        Application.Quit();
    }
}
