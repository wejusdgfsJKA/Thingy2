using Global;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class IntermediateSceneManager : MenuPDA
    {
        Mission selectedMission;
        [SerializeField] TextMeshProUGUI powerText;
        [SerializeField] Window mainWindow;
        private void Awake()
        {
            selectedMission = new FleetBattleMission(CalculateEnemyPoints(), 0);
        }
        private void OnEnable()
        {
            powerText.text = $"Coalition Power: {GameManager.PlayerPower}/{GlobalSettings.PlayerWinThreshold}\nEnemy Power: {GameManager.EnemyPower}/{GlobalSettings.EnemyWinThreshold}";
            OpenWindow(mainWindow);
        }
        public int CalculateEnemyPoints()
        {
            return (int)(((GameManager.PlayerPower + GameManager.EnemyPower) / 2) +
            Mathf.Max(GameManager.PlayerPower, GameManager.EnemyPower)) / 2;
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
    }
}