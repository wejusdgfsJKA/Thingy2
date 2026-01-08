using UnityEngine;
namespace Player.UI
{
    public class PauseMenuManager : MenuPDA
    {
        [SerializeField] Window menu;
        [SerializeField] GameObject boundingBoxParent;
        public int activeWindows => windows.Count;
        private void OnEnable()
        {
            menu.Deactivate();
            boundingBoxParent.SetActive(true);
        }
        private void OnDisable()
        {
            CloseAllWindows();
            menu.Deactivate();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void ToggleMenu()
        {
            if (menu.Active)
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
        public void OnExitToMenu()
        {
            GameManager.ExitToMenu();
        }
    }
}