using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class IntermediateSceneManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreText;
        private void OnEnable()
        {
            if (GameManager.Score != null) scoreText.text = GameManager.Score.ToString();
        }
        public void OnBeginMission()
        {
            GameManager.CurrentMission = new FleetBattleMission();
            Addressables.LoadSceneAsync(GlobalSettings.MainSceneAddress);
        }
        public void OnExit()
        {
            SceneManager.LoadScene(0);
        }
    }
}