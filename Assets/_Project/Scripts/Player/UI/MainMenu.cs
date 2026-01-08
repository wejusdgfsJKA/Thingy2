using Global;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Player.UI
{
    public class MainMenu : MenuPDA
    {
        [SerializeField] Canvas mainMenuCanvas, loadingCanvas;
        [SerializeField] Window menu, newGameWarning;
        [SerializeField] Button continueButton;
        private async void Start()
        {
            mainMenuCanvas.enabled = false;
            loadingCanvas.enabled = true;
            SceneManager.sceneLoaded += (s1, s2) =>
            {
                GameManager.StartMission();
            };
            var t1 = UnitManager.LoadAssets();
            var t2 = Weapons.WeaponManager.LoadAssets();
            await Task.WhenAll(t1, t2);
            loadingCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
            OpenWindow(menu);
            continueButton.interactable = GameManager.Load(GlobalSettings.GetSaveFilePath());
        }
        public void OnContinue()
        {
            if (GameManager.Load(GlobalSettings.GetSaveFilePath())) StartGame();
        }
        public void OnNewGame()
        {
            if (continueButton.interactable)
            {
                //display a warning
                OpenWindow(newGameWarning);
            }
            else StartNewGameConfirmed();
        }
        public void StartNewGameConfirmed()
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
            CloseAllWindows();
        }
        public override void CloseWindow()
        {
            if (windows.Count == 1)
            {
                //exiting the game
                Application.Quit();
                return;
            }
            base.CloseWindow();
        }
    }
}