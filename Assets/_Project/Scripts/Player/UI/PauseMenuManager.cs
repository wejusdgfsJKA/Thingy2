using UnityEngine;
using UnityEngine.SceneManagement;
namespace Player.UI
{
    public class PauseMenuManager : MenuManager
    {
        [SerializeField] GameObject menu;
        [SerializeField] GameObject boundingBoxParent;
        private void OnEnable()
        {
            menu.SetActive(false);
            boundingBoxParent.SetActive(true);
        }
        private void OnDisable()
        {
            CloseAllWindows();
            menu.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void ToggleMenu()
        {
            if (menu.activeSelf)
            {
                CloseAllWindows();
                if (boundingBoxParent) boundingBoxParent.SetActive(true);
            }
            else
            {
                if (boundingBoxParent) boundingBoxParent.SetActive(false);
                OpenWindow(menu);
            }
        }
        public void OnEndMission()
        {
            GameManager.ResumeGame();
            GameManager.AutoResolve();
        }
        public void OnExitToMenu()
        {
            GameManager.ResumeGame();
            SceneManager.LoadScene(0);
        }
    }
}