using Global;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class IntermediateSceneManager : MenuManager
    {
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] RectTransform powerBarParent, playerPowerBar, enemyPowerBar;
        private void OnEnable()
        {
            if (GameManager.Score != null) scoreText.text = GameManager.Score.ToString();
            float playerBarX = (GlobalSettings.PlayerWinThreshold + GameManager.CurrentPowerBalance) / (Mathf.Abs(GlobalSettings.PlayerLoseThreshold) + GlobalSettings.PlayerWinThreshold);
            float enemyBarX = (Mathf.Abs(GlobalSettings.PlayerLoseThreshold) - GameManager.CurrentPowerBalance) / (Mathf.Abs(GlobalSettings.PlayerLoseThreshold) + GlobalSettings.PlayerWinThreshold);
            playerPowerBar.localScale = new Vector3(playerBarX, playerPowerBar.localScale.y, playerPowerBar.localScale.z);
            enemyPowerBar.localScale = new Vector3(enemyBarX, enemyPowerBar.localScale.y, enemyPowerBar.localScale.z);
        }
        public void OnBeginMission()
        {
            GameManager.CurrentMission = new FleetBattleMission(1);
            Addressables.LoadSceneAsync(GlobalSettings.MainSceneAddress);
        }
        public void OnExit()
        {
            SceneManager.LoadScene(0);
        }
    }
}