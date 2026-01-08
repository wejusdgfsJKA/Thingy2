using Global;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class IntermediateSceneManager : MenuPDA
    {
        Mission selectedMission = new FleetBattleMission(1);
        [SerializeField] TextMeshProUGUI powerText;
        [SerializeField] Window mainWindow;
        private void OnEnable()
        {
            powerText.text = $"Coalition Power: {GameManager.PlayerPower}/{GlobalSettings.PlayerWinThreshold}\nEnemy Power: {GameManager.EnemyPower}/{GlobalSettings.EnemyWinThreshold}";
            OpenWindow(mainWindow);
        }
        public void OnBeginMission()
        {
            GameManager.CurrentMission = selectedMission;
            Addressables.LoadSceneAsync(GlobalSettings.MainSceneAddress);
        }
        public void OnExitConfirmed()
        {
            SceneManager.LoadScene(0);
        }
        public void OnEscapeKey()
        {
            if (windows.Count > 0)
            {
                CloseWindow();
            }
        }
        private void Update()
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject);
        }
    }
}