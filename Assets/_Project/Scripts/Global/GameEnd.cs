using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Global
{
    public class GameEnd : MonoBehaviour
    {
        [SerializeField] float opacityIncrease = 50;
        [SerializeField] TextMeshProUGUI endText;
        [SerializeField] Button mainMenuButton;
        private void Awake()
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }
        private void OnEnable()
        {
            if (GameManager.CurrentPowerBalance >= GlobalSettings.PlayerWinThreshold)
            {
                endText.text = "Victory!";
            }
            else
            {
                endText.text = "Defeat!";
            }
            endText.text += $"\nKills: {GameManager.PlayerKills}";
            endText.alpha = 0;
            mainMenuButton.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (endText.alpha < 1)
            {
                endText.alpha += opacityIncrease * Time.deltaTime;
                if (endText.alpha > .1)
                {
                    var tempColor = mainMenuButton.image.color;
                    tempColor.a = endText.alpha;
                    mainMenuButton.image.color = tempColor;
                    mainMenuButton.gameObject.SetActive(true);
                }
            }
        }
    }
}