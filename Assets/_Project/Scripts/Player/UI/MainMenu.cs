using Global;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Player.UI
{
    public class MainMenu : MenuManager
    {
        [SerializeField] Canvas mainMenuCanvas, loadingCanvas;
        [SerializeField] GameObject playMenu, newGameWarning;
        [SerializeField] Button continueButton;
        private void OnEnable()
        {
            Debug.Log("Main menu");
        }
        private async void Start()
        {
            SceneManager.sceneLoaded += (s1, s2) =>
            {
                GameManager.StartMission();
            };
            var t1 = ObjectManager.LoadAssets();
            var t2 = Weapons.WeaponManager.LoadAssets();
            await Task.WhenAll(t1, t2);
            loadingCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
        }
        public void OnPlay()
        {
            OpenWindow(playMenu);
            continueButton.interactable = GameSave.Load(GlobalSettings.GetSaveFilePath()) != null;
        }
        public void OnContinue()
        {
            GameManager.Load(GlobalSettings.GetSaveFilePath());
            StartGame();
        }
        public void OnNewGame()
        {
            if (continueButton.interactable)
            {
                //display a warning
                OpenWindow(newGameWarning);
            }
            else StartNewGame();
        }
        public void StartNewGame()
        {
            GameManager.BeginNewRun();
            StartGame();
        }
        public void StartGame()
        {
            Addressables.LoadSceneAsync(GlobalSettings.IntermediateSceneAddress);
        }
        public void OnExit()
        {
            Application.Quit();
        }
    }
}