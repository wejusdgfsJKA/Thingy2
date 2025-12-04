using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Canvas mainMenuCanvas, loadingCanvas;
        private async void Start()
        {
            SceneManager.sceneLoaded += (s1, s2) =>
            {
                GameManager.StartMission();
            };
            var t1 = ObjectManager.LoadAssets();
            var t2 = Weapons.RandomShitManager.LoadAssets();
            await Task.WhenAll(t1, t2);
            loadingCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
        }
        public void OnStart()
        {
            Addressables.LoadSceneAsync(GlobalSettings.IntermediateSceneAddress);
        }
        public void OnExit()
        {
            Application.Quit();
        }
    }
}